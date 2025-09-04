'use client';

export default function Background() {
  return (
    <>
      {/* Spider Web Background */}
      <div className="fixed inset-0 z-0">
        <svg className="w-full h-full opacity-5" viewBox="0 0 100 100" preserveAspectRatio="none">
          <defs>
            <pattern id="spiderweb-main" x="0" y="0" width="20" height="20" patternUnits="userSpaceOnUse">
              <path d="M10,0 L10,20 M0,10 L20,10 M2,2 L18,18 M18,2 L2,18"
                    stroke="white" strokeWidth="0.3" fill="none"/>
            </pattern>
          </defs>
          <rect width="100%" height="100%" fill="url(#spiderweb-main)"/>
        </svg>
      </div>

      {/* Background Effects */}
      <div className="fixed inset-0 opacity-20 z-0">
        <div className="absolute top-20 left-20 w-96 h-96 bg-spiderman-red/20 rounded-full blur-3xl animate-pulse"></div>
        <div className="absolute bottom-20 right-20 w-96 h-96 bg-spiderman-blue/20 rounded-full blur-3xl animate-pulse delay-1000"></div>
        <div className="absolute top-1/2 left-1/2 transform -translate-x-1/2 -translate-y-1/2 w-80 h-80 bg-spiderman-gold/10 rounded-full blur-3xl animate-pulse delay-2000"></div>
      </div>
    </>
  );
}
