import React, { createContext, useContext, useState } from "react";

export interface ShippingInfo {
  name: string;
  address: string;
  phone: string;
}

interface ShippingInfoContextType {
  shippingInfo: ShippingInfo;
  setShippingInfo: (info: ShippingInfo) => void;
}

const ShippingInfoContext = createContext<ShippingInfoContextType | undefined>(undefined);

export function ShippingInfoProvider({ children }: { children: React.ReactNode }) {
  const [shippingInfo, setShippingInfo] = useState<ShippingInfo>({ name: "", address: "", phone: "" });
  return (
    <ShippingInfoContext.Provider value={{ shippingInfo, setShippingInfo }}>
      {children}
    </ShippingInfoContext.Provider>
  );
}

export function useShippingInfo() {
  const ctx = useContext(ShippingInfoContext);
  if (!ctx) throw new Error("useShippingInfo must be used within ShippingInfoProvider");
  return ctx;
}
