'use client';

import { useState, useEffect } from 'react';
import { useRouter } from 'next/navigation';
import RegisterForm from '@/components/RegisterForm';

export default function RegisterPage() {
  const router = useRouter();

  useEffect(() => {
    // Check if user is already logged in
    if (typeof window !== 'undefined') {
      const token = localStorage.getItem('token');
      if (token) {
        router.push('/');
      }
    }
  }, [router]);

  const handleSwitchToLogin = () => {
    router.push('/login');
  };

  return (
    <div className="relative overflow-hidden">
      {/* Spider Web SVG Background */}
      <div className="fixed inset-0 z-0">
        <svg className="w-full h-full opacity-5" viewBox="0 0 100 100" preserveAspectRatio="none">
          <defs>
            <pattern id="spiderweb" x="0" y="0" width="20" height="20" patternUnits="userSpaceOnUse">
              <path d="M10,0 L10,20 M0,10 L20,10 M2,2 L18,18 M18,2 L2,18"
                  stroke="white" strokeWidth="0.5" fill="none"/>
            </pattern>
          </defs>
          <rect width="100%" height="100%" fill="url(#spiderweb)"/>
        </svg>
      </div>

      {/* Content */}
      <div className="relative z-10">
        <RegisterForm onSwitchToLogin={handleSwitchToLogin} />
      </div>
    </div>
  );
}
