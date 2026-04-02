# ☸️ Kubernetes Deployment Guide — Inventory Management System

## Prerequisites

| Tool | Purpose | Install |
|------|---------|---------|
| Docker Desktop | Build images | [docker.com](https://www.docker.com/products/docker-desktop) |
| kubectl | K8s CLI | Included with Docker Desktop |
| minikube *(optional)* | Local cluster | [minikube.sigs.k8s.io](https://minikube.sigs.k8s.io) |
| Nginx Ingress Controller | Routing | See below |

---

## Option A — Docker Compose (Simplest — Start Here)

Run all 3 services with a single command from the **repo root**:

```bash
# Build and start everything
docker-compose up --build

# Run in background
docker-compose up -d --build

# Stop and clean up
docker-compose down

# Stop + delete the MySQL volume (CLEARS ALL DATA)
docker-compose down -v
```

| Service  | URL |
|----------|-----|
| Frontend | http://localhost:3000 |
| API      | http://localhost:5000 |
| Swagger  | http://localhost:5000/swagger |
| Health   | http://localhost:5000/health |

---

## Option B — Kubernetes (Full Production Setup)

### Step 1 — Start a local cluster

**Docker Desktop (recommended for Windows):**
- Docker Desktop → Settings → Kubernetes → Enable Kubernetes ✓

**Or use minikube:**
```bash
minikube start --memory=4096 --cpus=2
eval $(minikube docker-env)   # Use minikube's Docker daemon
```

### Step 2 — Install Nginx Ingress Controller

**Docker Desktop:**
```bash
kubectl apply -f https://raw.githubusercontent.com/kubernetes/ingress-nginx/controller-v1.10.1/deploy/static/provider/cloud/deploy.yaml

# Wait for ingress controller to be ready
kubectl wait --namespace ingress-nginx \
  --for=condition=ready pod \
  --selector=app.kubernetes.io/component=controller \
  --timeout=120s
```

**minikube:**
```bash
minikube addons enable ingress
```

### Step 3 — Build Docker images

```bash
# From the repo root:

# Build the API image
docker build -t inventory-management-api:latest .

# Build the Frontend image
docker build -t inventory-management-frontend:latest ./inventory-frontend
```

### Step 4 — Deploy to Kubernetes

```bash
# Apply all manifests in order
kubectl apply -f k8s/namespace.yaml
kubectl apply -f k8s/mysql-secret.yaml
kubectl apply -f k8s/mysql-pvc.yaml
kubectl apply -f k8s/mysql-deployment.yaml
kubectl apply -f k8s/api-configmap.yaml
kubectl apply -f k8s/api-deployment.yaml
kubectl apply -f k8s/frontend-deployment.yaml
kubectl apply -f k8s/ingress.yaml
kubectl apply -f k8s/hpa.yaml

# ── Or apply everything at once ─────────────────────────────────────────────
kubectl apply -f k8s/
```

### Step 5 — Verify all pods are Running

```bash
# Watch pods start up (Ctrl+C to stop watching)
kubectl get pods -n inventory-system -w

# Expected output:
# NAME                        READY   STATUS    RESTARTS
# mysql-xxxx                  1/1     Running   0
# api-xxxx                    1/1     Running   0
# api-yyyy                    1/1     Running   0
# frontend-xxxx               1/1     Running   0
# frontend-yyyy               1/1     Running   0
```

### Step 6 — Configure local hosts file

Add to `C:\Windows\System32\drivers\etc\hosts` (run Notepad as Administrator):
```
127.0.0.1   inventory.local
```

For minikube, use the minikube IP instead:
```bash
minikube ip   # e.g., 192.168.49.2
# Then add: 192.168.49.2   inventory.local
```

### Step 7 — Access the application

| Service  | URL |
|----------|-----|
| Frontend | http://inventory.local |
| API      | http://inventory.local/api |
| Swagger  | http://inventory.local/swagger |
| Health   | http://inventory.local/health |

---

## Useful kubectl Commands

```bash
# View all resources in the namespace
kubectl get all -n inventory-system

# Check pod logs
kubectl logs -n inventory-system deployment/api --follow
kubectl logs -n inventory-system deployment/mysql

# Describe a pod (useful for debugging startup issues)
kubectl describe pod -n inventory-system -l app=api

# Check HPA status
kubectl get hpa -n inventory-system

# Scale manually
kubectl scale deployment api -n inventory-system --replicas=3

# Delete everything (keeps namespace and PVC)
kubectl delete -f k8s/ --ignore-not-found=true

# Delete everything including the namespace
kubectl delete namespace inventory-system
```

---

## Architecture Overview

```
                         ┌─────────────────────────────┐
                         │     Nginx Ingress Controller  │ :80
                         └───────────┬─────────┬────────┘
                                     │         │
                     /api, /swagger  │         │  /
                                     ▼         ▼
                              ┌──────────┐ ┌──────────┐
                              │  API svc │ │  FE svc  │
                              │  :8080   │ │  :80     │
                              └────┬─────┘ └──────────┘
                                   │ 2 replicas + HPA
                                   ▼
                              ┌──────────┐
                              │  MySQL   │
                              │  :3306   │
                              │  5Gi PVC │
                              └──────────┘
```

---

## Troubleshooting

| Problem | Fix |
|---------|-----|
| API pod CrashLoopBackOff | Check MySQL is ready: `kubectl logs -n inventory-system deployment/mysql` |
| Ingress not working | Ensure ingress controller is running: `kubectl get pods -n ingress-nginx` |
| Image pull error | Make sure you built images **after** running `eval $(minikube docker-env)` |
| `inventory.local` not resolving | Check hosts file entry |
| HPA shows `<unknown>` | Install metrics-server: `minikube addons enable metrics-server` |
