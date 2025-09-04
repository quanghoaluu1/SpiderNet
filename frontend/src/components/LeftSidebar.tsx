'use client';

import Image from 'next/image';
import { Users, Video, Store } from 'lucide-react';
import { User } from '@/types';

interface LeftSidebarProps {
  user: User;
}

export default function LeftSidebar({ user }: LeftSidebarProps) {
  return (
    <div className="lg:col-span-3">
      {/* User Profile Card */}
      <div className="glass-morphism rounded-lg shadow-xl p-4 mb-4 border border-white/10">
        <div className="flex items-center space-x-3 mb-4">
          <div className="bg-spiderman-blue p-1 rounded-full">
            <Image src="/icons/icon.png" alt={user.firstName} width={45} height={45} className="rounded-full" />
          </div>
          <div>
            <h3 className="font-semibold text-white">{user.firstName} {user.lastName}</h3>
            <p className="text-sm text-white/70">@{user.firstName.toLowerCase()}{user.lastName.toLowerCase()}</p>
          </div>
        </div>
        <div className="text-sm text-white/80">
          <p className="mb-2">🕷️ Your friendly neighborhood social network</p>
          <p>📧 {user.email}</p>
        </div>
      </div>

      {/* Quick Actions */}
      <div className="glass-morphism rounded-lg shadow-xl p-4 border border-white/10">
        <h3 className="font-semibold mb-3 text-white">Quick Actions</h3>
        <div className="space-y-2">
          <button className="flex items-center space-x-2 w-full text-left p-2 hover:bg-white/10 rounded transition-colors">
            <Users className="w-4 h-4 text-spiderman-blue" />
            <span className="text-sm text-white/90">Find Friends</span>
          </button>
          <button className="flex items-center space-x-2 w-full text-left p-2 hover:bg-white/10 rounded transition-colors">
            <Video className="w-4 h-4 text-spiderman-red" />
            <span className="text-sm text-white/90">Create Room</span>
          </button>
          <button className="flex items-center space-x-2 w-full text-left p-2 hover:bg-white/10 rounded transition-colors">
            <Store className="w-4 h-4 text-spiderman-gold" />
            <span className="text-sm text-white/90">Marketplace</span>
          </button>
        </div>
      </div>
    </div>
  );
}
