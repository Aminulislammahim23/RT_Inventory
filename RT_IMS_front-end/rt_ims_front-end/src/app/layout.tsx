import type { Metadata } from "next";
import { ProtectedLayout } from "@/components/ProtectedLayout";
import "./globals.css";

export const metadata: Metadata = {
  title: "RFID Yarn Store Management",
  description: "RFID based yarn warehouse management dashboard",
};

export default function RootLayout({
  children,
}: Readonly<{
  children: React.ReactNode;
}>) {
  return (
    <html lang="en" className="antialiased">
      <body>
        <ProtectedLayout>{children}</ProtectedLayout>
      </body>
    </html>
  );
}
