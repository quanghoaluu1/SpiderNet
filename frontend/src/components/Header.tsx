'use client';

import { useRouter } from 'next/navigation';
import Image from 'next/image';
import { Search, Bell, Home, Users, Video, Store } from 'lucide-react';
import { User } from '@/types';

interface HeaderProps {
  user: User;
  onLogout: () => void;
}

export default function Header({ user, onLogout }: HeaderProps) {
  const router = useRouter();

  return (
    <header className="glass-morphism sticky top-0 z-50 border-b border-white/10">
      <div className="max-w-6xl mx-auto px-4 py-3">
        <div className="flex items-center justify-between">
          {/* Logo and Search */}
          <div className="flex items-center space-x-4">
            <div className="flex items-center space-x-2">
                <Image 
                  src="/icons/icon_no_title.png" 
                  alt="SpiderNet" 
                  width={50} 
                  height={50} 
                  className="rounded-full cursor-pointer hover:opacity-80 transition-opacity" 
                  onClick={() => router.push('/')}
                />
              {/* <h1 className="text-2xl font-bold text-spiderman-red">SpiderNet</h1> */}
            </div>
            <div className="hidden md:flex items-center glass-morphism rounded-full px-4 py-2 w-80 border border-white/20">
              <Search className="w-5 h-5 text-white/70 mr-2" />
              <input
                type="text"
                placeholder="Search SpiderNet"
                className="bg-transparent outline-none flex-1 text-white placeholder-white/50"
              />
            </div>
          </div>

          {/* Navigation */}
          <div className="hidden md:flex items-center space-x-8">
            <Home 
              className="w-6 h-6 text-spiderman-red cursor-pointer hover:text-spiderman-dark-red transition-colors" 
              aria-label="Home"
              onClick={() => router.push('/')}
            />
            <Users 
              className="w-6 h-6 text-white/70 hover:text-spiderman-blue cursor-pointer transition-colors" 
              aria-label="Friends"
            />
            <Video 
              className="w-6 h-6 text-white/70 hover:text-spiderman-blue cursor-pointer transition-colors" 
              aria-label="Videos"
            />
            <Store 
              className="w-6 h-6 text-white/70 hover:text-spiderman-blue cursor-pointer transition-colors" 
              aria-label="Marketplace"
            />
          </div>

          {/* Right side */}
          <div className="flex items-center space-x-4">
            <Bell 
              className="w-6 h-6 text-white/70 hover:text-spiderman-gold cursor-pointer transition-colors" 
              aria-label="Notifications"
            />
            <div className="relative group">
              <div className="cursor-pointer py-2 px-2 rounded-lg hover:bg-white/5 transition-colors" title={`${user.firstName} ${user.lastName}`}>
                <div className="bg-spiderman-blue p-1 rounded-full">
                  <Image 
                    src={user.avatarUrl || "/icons/icon.png"} 
                    alt={user.firstName} 
                    width={32} 
                    height={32} 
                    className="rounded-full object-cover w-8 h-8" 
                  />
                </div>
              </div>
              <div className="absolute right-0 top-full w-48 bg-black/50 rounded-lg shadow-lg py-2 opacity-0 invisible group-hover:opacity-100 group-hover:visible transition-all duration-200 border border-white/20 z-50">
                <button
                  onClick={() => router.push('/profile')}
                  className="block w-full text-left px-4 py-2 text-sm text-white/90 hover:bg-white/10 transition-colors"
                >
                  Profile
                </button>
                <button
                  onClick={onLogout}
                  className="block w-full text-left px-4 py-2 text-sm text-white/90 hover:bg-white/10 transition-colors"
                >
                  Log out
                </button>
              </div>
            </div>
          </div>
        </div>
      </div>
    </header>
  );
}
