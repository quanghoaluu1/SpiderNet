'use client';

import Image from 'next/image';

export default function RightSidebar() {
  return (
    <div className="lg:col-span-3">
      {/* Sponsored Section */}
      <div className="glass-morphism rounded-lg shadow-xl p-4 mb-4 border border-white/10">
        <h3 className="font-semibold mb-3 text-white">Sponsored</h3>
        <div className="space-y-3">
          <div className="flex items-center space-x-3 p-2 hover:bg-white/5 rounded transition-colors">
            <div className="bg-spiderman-red p-1 rounded">
              <Image src="/icons/icon.png" alt="Ad" width={36} height={36} className="rounded" />
            </div>
            <div>
              <p className="text-sm font-medium text-white">Web-Slinging Gear</p>
              <p className="text-xs text-white/60">Get the latest spider tech!</p>
            </div>
          </div>
        </div>
      </div>

      {/* Friend Requests */}
      <div className="glass-morphism rounded-lg shadow-xl p-4 border border-white/10">
        <h3 className="font-semibold mb-3 text-white">Friend Requests</h3>
        <div className="space-y-3">
          <div className="flex items-center justify-between p-2 hover:bg-white/5 rounded transition-colors">
            <div className="flex items-center space-x-3">
              <div className="bg-spiderman-gold p-1 rounded-full">
                <Image src="/icons/icon.png" alt="Friend" width={36} height={36} className="rounded-full" />
              </div>
              <div>
                <p className="text-sm font-medium text-white">Mary Jane Watson</p>
                <p className="text-xs text-white/60">2 mutual friends</p>
              </div>
            </div>
            <div className="flex space-x-2">
              <button className="bg-spiderman-blue text-white px-3 py-1 rounded text-xs hover:bg-spiderman-light-blue transition-colors">
                Accept
              </button>
              <button className="bg-white/20 text-white px-3 py-1 rounded text-xs hover:bg-white/30 transition-colors">
                Delete
              </button>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
}
