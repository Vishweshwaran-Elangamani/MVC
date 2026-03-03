import React from "react";
import logo from "../../assets/react.svg";

export default function Logo({ size = 28, withText = true }) {
  return (
    <div className="row" style={{ gap: 10 }}>
      <img src={logo} width={size} height={size} alt="BubbleApp logo" />
      {withText && <strong style={{ fontSize: 18 }}>BubbleApp</strong>}
    </div>
  );
}