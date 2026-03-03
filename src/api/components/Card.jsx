import React from "react";

export default function Card({ title, actions, children }) {
  return (
    <section className="card">
      <div className="row" style={{ justifyContent: "space-between", marginBottom: 10 }}>
        {title ? <h3>{title}</h3> : <span />}
        {actions}
      </div>
      {children}
    </section>
  );
}