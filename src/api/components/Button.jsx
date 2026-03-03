import React from "react";

export default function Button({ kind="default", className="", ...props }) {
  const cls = ["btn", kind === "primary" ? "primary" : kind === "ghost" ? "ghost" : "", className].join(" ");
  return <button className={cls} {...props} />;
}