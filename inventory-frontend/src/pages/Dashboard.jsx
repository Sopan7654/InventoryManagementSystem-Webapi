// src/pages/Dashboard.jsx
import { useState, useEffect } from "react";
import { Link } from "react-router-dom";
import {
  AreaChart,
  Area,
  BarChart,
  Bar,
  XAxis,
  YAxis,
  Tooltip,
  ResponsiveContainer,
  CartesianGrid,
  Cell,
} from "recharts";
import {
  Package,
  Building2,
  TrendingDown,
  Layers,
  ArrowRight,
  AlertTriangle,
  CheckCircle2,
} from "lucide-react";
import {
  getStockLevels,
  getLowStock,
  getProducts,
  getWarehouses,
  getTransactions,
} from "../api/inventoryApi";
import LoadingSpinner from "../components/LoadingSpinner";

const IN_TYPES = ["PURCHASE", "TRANSFER_IN", "ADJUSTMENT"];
const OUT_TYPES = ["SALE", "TRANSFER_OUT", "HOLD"];

// Aggregate raw transactions into last-6-month buckets for the area chart
function buildTrendData(transactions) {
  const months = {};
  const now = new Date();
  for (let i = 5; i >= 0; i--) {
    const d = new Date(now.getFullYear(), now.getMonth() - i, 1);
    const key = d.toLocaleString("default", { month: "short" });
    months[key] = { month: key, in: 0, out: 0 };
  }
  transactions.forEach((tx) => {
    const d = new Date(tx.transactionDate);
    const key = d.toLocaleString("default", { month: "short" });
    if (!months[key]) return;
    const qty = Math.abs(tx.quantity || 0);
    if (IN_TYPES.includes(tx.transactionType)) months[key].in += qty;
    if (OUT_TYPES.includes(tx.transactionType)) months[key].out += qty;
  });
  return Object.values(months);
}

const COLORS = ["#6366f1", "#8b5cf6", "#a78bfa", "#c4b5fd"];

function StatCard({ icon: Icon, label, value, sub, color, to }) {
  const styles = {
    purple: {
      bg: "hsl(238 84% 67% / 0.1)",
      icon: "hsl(238 84% 67%)",
      border: "hsl(238 84% 67% / 0.2)",
    },
    green: {
      bg: "hsl(142 71% 45% / 0.1)",
      icon: "hsl(142 71% 45%)",
      border: "hsl(142 71% 45% / 0.2)",
    },
    orange: {
      bg: "hsl(25 95% 53% / 0.1)",
      icon: "hsl(25 95% 53%)",
      border: "hsl(25 95% 53% / 0.2)",
    },
    red: {
      bg: "hsl(0 72% 51% / 0.1)",
      icon: "hsl(0 72% 51%)",
      border: "hsl(0 72% 51% / 0.2)",
    },
  };
  const s = styles[color] || styles.purple;

  const card = (
    <div className="stat-card anim-count-up" style={{ borderColor: s.border }}>
      <div
        style={{
          width: 40,
          height: 40,
          borderRadius: 10,
          background: s.bg,
          display: "flex",
          alignItems: "center",
          justifyContent: "center",
          marginBottom: "0.875rem",
        }}
      >
        <Icon size={18} color={s.icon} strokeWidth={2} />
      </div>
      <p
        style={{
          fontSize: "1.875rem",
          fontWeight: 800,
          letterSpacing: "-0.04em",
          lineHeight: 1,
        }}
      >
        {value}
      </p>
      <p
        style={{
          fontSize: "0.875rem",
          fontWeight: 500,
          color: "hsl(var(--foreground))",
          marginTop: "0.25rem",
        }}
      >
        {label}
      </p>
      {sub && (
        <p
          style={{
            fontSize: "0.75rem",
            color: "hsl(var(--muted-foreground))",
            marginTop: "0.25rem",
          }}
        >
          {sub}
        </p>
      )}
    </div>
  );

  return to ? (
    <Link to={to} style={{ textDecoration: "none" }}>
      {card}
    </Link>
  ) : (
    card
  );
}

export default function Dashboard() {
  const [data, setData] = useState(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    Promise.all([
      getStockLevels().catch(() => ({ data: [] })),
      getLowStock().catch(() => ({ data: [] })),
      getProducts().catch(() => ({ data: [] })),
      getWarehouses().catch(() => ({ data: [] })),
      getTransactions(null, 500).catch(() => ({ data: [] })),
    ]).then(([stock, lowStock, products, warehouses, txns]) => {
      setData({
        stockItems: stock.data || [],
        lowStockItems: lowStock.data || [],
        products: products.data || [],
        warehouses: warehouses.data || [],
        trendData: buildTrendData(txns.data || []),
      });
      setLoading(false);
    });
  }, []);

  if (loading) return <LoadingSpinner text="Loading dashboard…" />;

  const totalProducts = data.products.length;
  const activeProducts = data.products.filter((p) => p.isActive).length;
  const totalWarehouses = data.warehouses.length;
  const lowStockCount = data.lowStockItems.length;
  const totalQty = data.stockItems.reduce(
    (s, i) => s + (i.quantityOnHand || 0),
    0,
  );

  // Stock by warehouse for bar chart
  const warehouseData = data.warehouses.slice(0, 6).map((w) => ({
    name:
      w.warehouseName?.slice(0, 10) + (w.warehouseName?.length > 10 ? "…" : ""),
    qty: data.stockItems
      .filter((s) => s.warehouseId === w.warehouseId)
      .reduce((s, i) => s + (i.quantityOnHand || 0), 0),
  }));

  return (
    <div className="anim-fade-in">
      {/* Header */}
      <div style={{ marginBottom: "2rem" }}>
        <h1 className="page-title">
          Inventory <span className="gradient-text">Dashboard</span>
        </h1>
        <p className="page-subtitle">
          Here's your inventory snapshot for today
        </p>
      </div>

      {/* KPI Cards */}
      <div
        className="anim-stagger"
        style={{
          display: "grid",
          gridTemplateColumns: "repeat(auto-fit, minmax(200px, 1fr))",
          gap: "1rem",
          marginBottom: "1.75rem",
        }}
      >
        <StatCard
          icon={Package}
          label="Total Products"
          value={totalProducts}
          sub={`${activeProducts} active`}
          color="purple"
          to="/products"
        />
        <StatCard
          icon={Building2}
          label="Warehouses"
          value={totalWarehouses}
          sub="active locations"
          color="green"
          to="/warehouses"
        />
        <StatCard
          icon={Layers}
          label="Units on Hand"
          value={totalQty.toLocaleString()}
          sub="across all warehouses"
          color="orange"
          to="/inventory"
        />
        <StatCard
          icon={TrendingDown}
          label="Low Stock Alerts"
          value={lowStockCount}
          sub={lowStockCount > 0 ? "need reorder" : "all levels OK"}
          color={lowStockCount > 0 ? "red" : "green"}
          to="/reports"
        />
      </div>

      {/* Charts row */}
      <div
        style={{
          display: "grid",
          gridTemplateColumns: "1fr 1fr",
          gap: "1rem",
          marginBottom: "1.5rem",
        }}
      >
        {/* Area chart — stock movement trend */}
        <div className="card" style={{ padding: "1.25rem" }}>
          <div
            style={{
              display: "flex",
              alignItems: "flex-start",
              justifyContent: "space-between",
              marginBottom: "1.25rem",
            }}
          >
            <div>
              <p style={{ fontWeight: 700, fontSize: "0.9375rem" }}>
                Stock Movement
              </p>
              <p
                style={{
                  fontSize: "0.75rem",
                  color: "hsl(var(--muted-foreground))",
                }}
              >
                Last 6 months
              </p>
            </div>
            <span className="badge badge-default">Trend</span>
          </div>
          <ResponsiveContainer width="100%" height={180}>
            <AreaChart
              data={data.trendData}
              margin={{ top: 0, right: 0, left: -20, bottom: 0 }}
            >
              <defs>
                <linearGradient id="colorIn" x1="0" y1="0" x2="0" y2="1">
                  <stop offset="5%" stopColor="#6366f1" stopOpacity={0.3} />
                  <stop offset="95%" stopColor="#6366f1" stopOpacity={0} />
                </linearGradient>
                <linearGradient id="colorOut" x1="0" y1="0" x2="0" y2="1">
                  <stop offset="5%" stopColor="#a78bfa" stopOpacity={0.2} />
                  <stop offset="95%" stopColor="#a78bfa" stopOpacity={0} />
                </linearGradient>
              </defs>
              <CartesianGrid strokeDasharray="3 3" vertical={false} />
              <XAxis dataKey="month" tick={{ fontSize: 11 }} />
              <YAxis tick={{ fontSize: 11 }} />
              <Tooltip />
              <Area
                type="monotone"
                dataKey="in"
                stroke="#6366f1"
                strokeWidth={2}
                fill="url(#colorIn)"
                name="Stock In"
              />
              <Area
                type="monotone"
                dataKey="out"
                stroke="#a78bfa"
                strokeWidth={2}
                fill="url(#colorOut)"
                name="Stock Out"
              />
            </AreaChart>
          </ResponsiveContainer>
        </div>

        {/* Bar chart — stock by warehouse */}
        <div className="card" style={{ padding: "1.25rem" }}>
          <div
            style={{
              display: "flex",
              alignItems: "flex-start",
              justifyContent: "space-between",
              marginBottom: "1.25rem",
            }}
          >
            <div>
              <p style={{ fontWeight: 700, fontSize: "0.9375rem" }}>
                By Warehouse
              </p>
              <p
                style={{
                  fontSize: "0.75rem",
                  color: "hsl(var(--muted-foreground))",
                }}
              >
                Units on hand
              </p>
            </div>
            <Link
              to="/warehouses"
              style={{
                display: "flex",
                alignItems: "center",
                gap: "0.25rem",
                fontSize: "0.75rem",
                color: "hsl(var(--primary))",
                textDecoration: "none",
                fontWeight: 600,
              }}
            >
              View all <ArrowRight size={12} />
            </Link>
          </div>
          {warehouseData.length > 0 ? (
            <ResponsiveContainer width="100%" height={180}>
              <BarChart
                data={warehouseData}
                margin={{ top: 0, right: 0, left: -20, bottom: 0 }}
              >
                <CartesianGrid strokeDasharray="3 3" vertical={false} />
                <XAxis dataKey="name" tick={{ fontSize: 10 }} />
                <YAxis tick={{ fontSize: 11 }} />
                <Tooltip />
                <Bar dataKey="qty" name="Units" radius={[4, 4, 0, 0]}>
                  {warehouseData.map((_, i) => (
                    <Cell key={i} fill={COLORS[i % COLORS.length]} />
                  ))}
                </Bar>
              </BarChart>
            </ResponsiveContainer>
          ) : (
            <div className="empty-state" style={{ padding: "2rem" }}>
              <span className="empty-icon">🏢</span>
              <p className="empty-body">No warehouse data</p>
            </div>
          )}
        </div>
      </div>

      {/* Low Stock / All OK */}
      {data.lowStockItems.length > 0 ? (
        <div className="card" style={{ overflow: "hidden" }}>
          <div
            style={{
              display: "flex",
              alignItems: "center",
              justifyContent: "space-between",
              padding: "1rem 1.25rem",
              borderBottom: "1px solid hsl(var(--border))",
            }}
          >
            <div
              style={{ display: "flex", alignItems: "center", gap: "0.625rem" }}
            >
              <AlertTriangle size={16} color="hsl(0 72% 51%)" strokeWidth={2} />
              <span style={{ fontWeight: 700, fontSize: "0.9375rem" }}>
                Low Stock Alerts
              </span>
              <span className="badge badge-danger">
                {data.lowStockItems.length}
              </span>
            </div>
            <Link
              to="/reports"
              style={{
                display: "flex",
                alignItems: "center",
                gap: "0.25rem",
                fontSize: "0.8125rem",
                color: "hsl(var(--primary))",
                textDecoration: "none",
                fontWeight: 600,
              }}
            >
              View all <ArrowRight size={14} />
            </Link>
          </div>
          <div className="data-table-wrap">
            <table className="data-table">
              <thead>
                <tr>
                  <th>Product</th>
                  <th>Warehouse</th>
                  <th style={{ textAlign: "right" }}>On Hand</th>
                  <th style={{ textAlign: "right" }}>Reorder At</th>
                </tr>
              </thead>
              <tbody>
                {data.lowStockItems.slice(0, 7).map((item, i) => (
                  <tr key={i}>
                    <td
                      style={{
                        fontWeight: 600,
                        color: "hsl(var(--foreground))",
                      }}
                    >
                      {item.productName}
                    </td>
                    <td>{item.warehouseName}</td>
                    <td style={{ textAlign: "right" }}>
                      <span className="badge badge-danger">
                        {item.quantityOnHand}
                      </span>
                    </td>
                    <td
                      style={{
                        textAlign: "right",
                        fontFamily: "'JetBrains Mono', monospace",
                        fontSize: "0.8125rem",
                      }}
                    >
                      {item.reorderLevel}
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        </div>
      ) : (
        <div
          className="card"
          style={{
            display: "flex",
            alignItems: "center",
            gap: "1rem",
            padding: "1.5rem 1.75rem",
          }}
        >
          <div
            style={{
              width: 40,
              height: 40,
              borderRadius: 10,
              background: "hsl(142 71% 45% / 0.12)",
              display: "flex",
              alignItems: "center",
              justifyContent: "center",
            }}
          >
            <CheckCircle2 size={20} color="hsl(142 71% 45%)" strokeWidth={2} />
          </div>
          <div>
            <p style={{ fontWeight: 700, fontSize: "0.9375rem" }}>
              All stock levels are healthy
            </p>
            <p
              style={{
                fontSize: "0.8125rem",
                color: "hsl(var(--muted-foreground))",
                marginTop: "0.125rem",
              }}
            >
              No items are below their reorder threshold
            </p>
          </div>
        </div>
      )}

      {/* Quick action shortcuts */}
      <div
        style={{
          display: "grid",
          gridTemplateColumns: "repeat(auto-fit, minmax(170px, 1fr))",
          gap: "0.75rem",
          marginTop: "1.5rem",
        }}
      >
        {[
          {
            to: "/inventory",
            icon: Layers,
            label: "Stock Operations",
            color: "#6366f1",
          },
          {
            to: "/purchase-orders",
            icon: Package,
            label: "Purchase Orders",
            color: "#8b5cf6",
          },
          {
            to: "/reports",
            icon: TrendingDown,
            label: "View Reports",
            color: "#a78bfa",
          },
        ].map(({ to, icon: Icon, label, color }) => (
          <Link key={to} to={to} style={{ textDecoration: "none" }}>
            <div
              className="card card-hover"
              style={{
                padding: "1rem",
                display: "flex",
                alignItems: "center",
                gap: "0.75rem",
                cursor: "pointer",
              }}
            >
              <div
                style={{
                  width: 32,
                  height: 32,
                  borderRadius: 8,
                  background: color + "20",
                  display: "flex",
                  alignItems: "center",
                  justifyContent: "center",
                  flexShrink: 0,
                }}
              >
                <Icon size={15} color={color} strokeWidth={2} />
              </div>
              <span style={{ fontSize: "0.8125rem", fontWeight: 600 }}>
                {label}
              </span>
              <ArrowRight
                size={14}
                style={{
                  marginLeft: "auto",
                  color: "hsl(var(--muted-foreground))",
                }}
              />
            </div>
          </Link>
        ))}
      </div>
    </div>
  );
}
