// src/pages/Login.jsx
import { useState } from "react";
import { useNavigate } from "react-router-dom";
import { loginUser, registerUser } from "../api/inventoryApi";
import {
  Eye,
  EyeOff,
  Lock,
  User,
  Mail,
  Package,
  Boxes,
  BarChart3,
  ShieldCheck,
  UserPlus,
} from "lucide-react";

const FEATURES = [
  {
    icon: Boxes,
    title: "Real-time Inventory",
    desc: "Track stock levels across all warehouses instantly",
  },
  {
    icon: BarChart3,
    title: "Advanced Analytics",
    desc: "ABC analysis, turnover & aging reports",
  },
  {
    icon: ShieldCheck,
    title: "Secure & Reliable",
    desc: "Enterprise-grade role-based access control",
  },
];

/* ── Reusable input ── */
function Field({
  id,
  label,
  type = "text",
  Icon,
  placeholder,
  value,
  onChange,
  onBlur,
  error,
  rightSlot,
  autoComplete,
  autoFocus,
}) {
  return (
    <div className="auth-field">
      <label htmlFor={id} className="auth-field-label">
        {label}
      </label>
      <div className="auth-input-wrap">
        <span className="auth-input-icon">
          <Icon size={14} />
        </span>
        <input
          id={id}
          type={type}
          value={value}
          placeholder={placeholder}
          onChange={onChange}
          onBlur={onBlur}
          autoComplete={autoComplete}
          autoFocus={autoFocus}
          className={`auth-input${rightSlot ? " has-right" : ""}${error ? " is-error" : ""}`}
        />
        {rightSlot && rightSlot}
      </div>
      {error && <span className="auth-field-err">{error}</span>}
    </div>
  );
}

/* ── Eye toggle ── */
function Eye2({ show, onToggle }) {
  return (
    <button
      type="button"
      className="auth-eye"
      tabIndex={-1}
      onClick={onToggle}
      aria-label={show ? "Hide" : "Show"}
    >
      {show ? <EyeOff size={14} /> : <Eye size={14} />}
    </button>
  );
}

/* ── Submit button ── */
function Btn({ label, loading, id, onClick }) {
  return (
    <button
      type={onClick ? "button" : "submit"}
      id={id}
      disabled={loading}
      onClick={onClick}
      className="auth-btn"
    >
      {loading ? <span className="auth-spinner" /> : label}
    </button>
  );
}

/* ── Divider ── */
function Divider() {
  return (
    <div className="auth-divider">
      <span className="auth-divider-line" />
      <span className="auth-divider-text">or</span>
      <span className="auth-divider-line" />
    </div>
  );
}

/* ═══════════════════════════════════════════════════
   ROOT
   ═══════════════════════════════════════════════════ */
export default function AuthPage() {
  const navigate = useNavigate();
  const [mode, setMode] = useState("login");

  return (
    <div className="auth-root">
      {/* ══ LEFT ══ */}
      <div className="auth-left">
        <div className="auth-glow auth-glow-1" />
        <div className="auth-glow auth-glow-2" />

        {/* Logo */}
        <div className="auth-logo">
          <div className="auth-logo-icon">
            <Package size={17} color="#000" strokeWidth={2.5} />
          </div>
          <span className="auth-logo-name">InventoryManagementSystem</span>
        </div>

        {/* Centered body */}
        <div className="auth-left-body">
          <p className="auth-eyebrow">
            Enterprise Inventory Management Platform
          </p>

          <h1 className="auth-headline">
            The smarter way
            <br />
            to manage your
            <br />
            <span className="auth-headline-grad">inventory.</span>
          </h1>

          <p className="auth-subline">
            One platform to track, analyse and optimise every unit across your
            entire supply chain.
          </p>

          <div className="auth-features">
            {FEATURES.map(({ icon: Icon, title, desc }) => (
              <div key={title} className="auth-feature-item">
                <div className="auth-feature-icon">
                  <Icon size={14} strokeWidth={2} />
                </div>
                <div>
                  <p className="auth-feature-title">{title}</p>
                  <p className="auth-feature-desc">{desc}</p>
                </div>
              </div>
            ))}
          </div>
        </div>
      </div>

      {/* ══ RIGHT ══ */}
      <div className="auth-right">
        <div className="auth-form-wrap" key={mode}>
          {mode === "login" ? (
            <LoginForm navigate={navigate} onSwitch={() => setMode("signup")} />
          ) : (
            <SignupForm navigate={navigate} onSwitch={() => setMode("login")} />
          )}
        </div>
      </div>
    </div>
  );
}

/* ═══════════════════════════════════════════════════
   LOGIN FORM
   ═══════════════════════════════════════════════════ */
function LoginForm({ navigate, onSwitch }) {
  const [form, setForm] = useState({ username: "", password: "" });
  const [show, setShow] = useState(false);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState("");
  const [touched, setTouched] = useState({});

  const set = (k, v) => {
    setForm((f) => ({ ...f, [k]: v }));
    setError("");
  };
  const touch = (k) => setTouched((t) => ({ ...t, [k]: true }));

  const submit = async (e) => {
    e.preventDefault();
    setTouched({ username: true, password: true });
    if (!form.username.trim() || !form.password) {
      setError("Please fill in all fields.");
      return;
    }
    setLoading(true);
    setError("");
    try {
      await loginUser(form.username.trim(), form.password);
      navigate("/");
    } catch (err) {
      setError(err.message || "Login failed. Please check your credentials.");
    } finally {
      setLoading(false);
    }
  };

  return (
    <>
      <div className="auth-form-icon">
        <Lock size={20} color="#818cf8" strokeWidth={2} />
      </div>
      <h2 className="auth-form-title">Welcome back</h2>
      <p className="auth-form-sub">Sign in to continue to IMS</p>

      <form onSubmit={submit} noValidate>
        <div className="auth-fields">
          <Field
            id="l-user"
            label="Username"
            Icon={User}
            placeholder="Your username"
            value={form.username}
            onChange={(e) => set("username", e.target.value)}
            onBlur={() => touch("username")}
            error={
              touched.username && !form.username.trim()
                ? "Username is required"
                : ""
            }
            autoComplete="username"
            autoFocus
          />

          <Field
            id="l-pass"
            label="Password"
            type={show ? "text" : "password"}
            Icon={Lock}
            placeholder="Your password"
            value={form.password}
            onChange={(e) => set("password", e.target.value)}
            onBlur={() => touch("password")}
            error={
              touched.password && !form.password ? "Password is required" : ""
            }
            rightSlot={<Eye2 show={show} onToggle={() => setShow((s) => !s)} />}
            autoComplete="current-password"
          />

          <label className="auth-remember">
            <input type="checkbox" /> Remember me for 30 days
          </label>

          {error && <p className="auth-banner-err">⚠ {error}</p>}

          <Btn label="Sign In" loading={loading} id="login-submit" />
        </div>
      </form>

      <Divider />

      <p className="auth-switch">
        Don't have an account?{" "}
        <button type="button" className="auth-switch-btn" onClick={onSwitch}>
          Sign up
        </button>
      </p>

      <div className="auth-tech">
        {["ASP.NET Core 8", "·", "MediatR CQRS", "·", "MySQL"].map((b, i) => (
          <span key={i}>{b}</span>
        ))}
      </div>
    </>
  );
}

/* ═══════════════════════════════════════════════════
   SIGNUP FORM
   ═══════════════════════════════════════════════════ */
function SignupForm({ navigate, onSwitch }) {
  const [form, setForm] = useState({
    username: "",
    email: "",
    password: "",
    confirm: "",
  });
  const [showP, setShowP] = useState(false);
  const [showC, setShowC] = useState(false);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState("");
  const [touched, setTouched] = useState({});
  const [done, setDone] = useState(false);

  const set = (k, v) => {
    setForm((f) => ({ ...f, [k]: v }));
    setError("");
  };
  const touch = (k) => setTouched((t) => ({ ...t, [k]: true }));

  const validate = () => {
    if (form.username.trim().length < 3)
      return "Username must be at least 3 characters.";
    if (!/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(form.email))
      return "Enter a valid email address.";
    if (form.password.length < 8)
      return "Password must be at least 8 characters.";
    if (form.password !== form.confirm) return "Passwords do not match.";
    return "";
  };

  const submit = async (e) => {
    e.preventDefault();
    setTouched({ username: true, email: true, password: true, confirm: true });
    const msg = validate();
    if (msg) {
      setError(msg);
      return;
    }
    setLoading(true);
    setError("");
    try {
      await registerUser(form.username.trim(), form.email.trim(), form.password);
      setDone(true);
    } catch (err) {
      setError(err.message || "Registration failed. Please try again.");
    } finally {
      setLoading(false);
    }
  };

  const strength = (() => {
    const p = form.password;
    if (!p) return 0;
    return [
      p.length >= 8,
      /[A-Z]/.test(p),
      /[0-9]/.test(p),
      /[^A-Za-z0-9]/.test(p),
    ].filter(Boolean).length;
  })();
  const SC = ["", "#ef4444", "#f59e0b", "#6366f1", "#22c55e"][strength];
  const SL = ["", "Weak", "Fair", "Good", "Strong"][strength];

  if (done)
    return (
      <div className="auth-success">
        <h2 className="auth-form-title">Account created!</h2>
        <p className="auth-form-sub">
          Your account is ready. Sign in to get started.
        </p>
        <Btn
          label="Go to Sign In"
          loading={false}
          id="goto-login"
          onClick={onSwitch}
        />
      </div>
    );

  return (
    <>
      <div className="auth-form-icon">
        <UserPlus size={20} color="#818cf8" strokeWidth={2} />
      </div>
      <h2 className="auth-form-title">Create account</h2>
      <p className="auth-form-sub">Set up your IMS account</p>

      <form onSubmit={submit} noValidate>
        <div className="auth-fields">
          <Field
            id="su-user"
            label="Username"
            Icon={User}
            placeholder="Choose a username"
            value={form.username}
            onChange={(e) => set("username", e.target.value)}
            onBlur={() => touch("username")}
            error={
              touched.username && form.username.trim().length < 3
                ? "At least 3 characters required"
                : ""
            }
            autoComplete="username"
            autoFocus
          />

          <Field
            id="su-email"
            label="Email"
            type="email"
            Icon={Mail}
            placeholder="you@example.com"
            value={form.email}
            onChange={(e) => set("email", e.target.value)}
            onBlur={() => touch("email")}
            error={
              touched.email && !/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(form.email)
                ? "Valid email required"
                : ""
            }
            autoComplete="email"
          />

          {/* Password + strength */}
          <div className="auth-field">
            <label htmlFor="su-pass" className="auth-field-label">
              Password
            </label>
            <div className="auth-input-wrap">
              <span className="auth-input-icon">
                <Lock size={14} />
              </span>
              <input
                id="su-pass"
                type={showP ? "text" : "password"}
                value={form.password}
                onChange={(e) => set("password", e.target.value)}
                onBlur={() => touch("password")}
                placeholder="At least 8 characters"
                autoComplete="new-password"
                className={`auth-input has-right${touched.password && form.password.length < 8 && form.password ? " is-error" : ""}`}
              />
              <Eye2 show={showP} onToggle={() => setShowP((s) => !s)} />
            </div>
            {form.password && (
              <div className="auth-strength">
                <div className="auth-strength-bars">
                  {[1, 2, 3, 4].map((n) => (
                    <div
                      key={n}
                      className="auth-strength-bar"
                      style={{ background: n <= strength ? SC : undefined }}
                    />
                  ))}
                </div>
                <span className="auth-strength-label" style={{ color: SC }}>
                  {SL}
                </span>
              </div>
            )}
          </div>

          <Field
            id="su-confirm"
            label="Confirm Password"
            type={showC ? "text" : "password"}
            Icon={Lock}
            placeholder="Re-enter password"
            value={form.confirm}
            onChange={(e) => set("confirm", e.target.value)}
            onBlur={() => touch("confirm")}
            error={
              touched.confirm && form.password !== form.confirm
                ? "Passwords do not match"
                : ""
            }
            rightSlot={
              <Eye2 show={showC} onToggle={() => setShowC((s) => !s)} />
            }
            autoComplete="new-password"
          />

          {error && <p className="auth-banner-err">⚠ {error}</p>}

          <Btn label="Create Account" loading={loading} id="signup-submit" />
        </div>
      </form>

      <Divider />

      <p className="auth-switch">
        Already have an account?{" "}
        <button type="button" className="auth-switch-btn" onClick={onSwitch}>
          Sign in
        </button>
      </p>
    </>
  );
}
