# ─────────────────────────────────────────────────────────────────────────────
# Stage 1: Build React app with Node.js
# ─────────────────────────────────────────────────────────────────────────────
FROM node:20-alpine AS build

WORKDIR /app

# Copy package files first for better layer caching
COPY package.json package-lock.json ./

RUN npm ci --silent

# Copy source code
COPY . .

# Build production bundle
RUN npm run build

# ─────────────────────────────────────────────────────────────────────────────
# Stage 2: Serve with lightweight Nginx
# ─────────────────────────────────────────────────────────────────────────────
FROM nginx:alpine AS final

# Remove default nginx content
RUN rm -rf /usr/share/nginx/html/*

# Copy built React app from build stage
COPY --from=build /app/dist /usr/share/nginx/html

# Copy our custom nginx config
COPY nginx.conf /etc/nginx/conf.d/default.conf

# Expose HTTP port
EXPOSE 80

# Run nginx in the foreground
CMD ["nginx", "-g", "daemon off;"]
