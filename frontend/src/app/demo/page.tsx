"use client";

import Link from 'next/link';
import { Button } from '@/components/ui/button';
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
import { User, ArrowRight, Shield, Star } from 'lucide-react';

export default function DemoPage() {
  return (
    <div className="min-h-screen bg-gradient-to-br from-slate-900 via-red-950 to-blue-950 p-8">
      <div className="max-w-4xl mx-auto">
        <div className="text-center mb-12">
          <h1 className="text-5xl font-bold text-white mb-4 bg-gradient-to-r from-red-400 to-blue-400 bg-clip-text text-transparent">
            SpiderNet Profile System
          </h1>
          <p className="text-xl text-slate-300">
            A Spider-Man themed social media profile system built with Next.js, Tailwind CSS, and shadcn/ui
          </p>
        </div>

        <div className="grid grid-cols-1 md:grid-cols-2 gap-8 mb-12">
          <Card className="bg-slate-800/90 border-red-500/20 backdrop-blur-sm">
            <CardHeader>
              <CardTitle className="text-red-400 flex items-center">
                <User className="h-6 w-6 mr-2" />
                My Profile
              </CardTitle>
            </CardHeader>
            <CardContent>
              <p className="text-slate-300 mb-4">
                View and edit your own profile with full functionality including profile updates and password changes.
              </p>
              <Link href="/profile">
                <Button className="w-full bg-red-600 hover:bg-red-700">
                  View My Profile
                  <ArrowRight className="h-4 w-4 ml-2" />
                </Button>
              </Link>
            </CardContent>
          </Card>

          <Card className="bg-slate-800/90 border-blue-500/20 backdrop-blur-sm">
            <CardHeader>
              <CardTitle className="text-blue-400 flex items-center">
                <Shield className="h-6 w-6 mr-2" />
                User Profile
              </CardTitle>
            </CardHeader>
            <CardContent>
              <p className="text-slate-300 mb-4">
                View another user&apos;s profile (read-only mode) using a sample user ID.
              </p>
              <Link href="/profile/123e4567-e89b-12d3-a456-426614174000">
                <Button variant="outline" className="w-full border-blue-500/50 hover:bg-blue-500/10">
                  View Sample User
                  <ArrowRight className="h-4 w-4 ml-2" />
                </Button>
              </Link>
            </CardContent>
          </Card>
        </div>

        <Card className="bg-slate-800/90 border-yellow-500/20 backdrop-blur-sm">
          <CardHeader>
            <CardTitle className="text-yellow-400 flex items-center">
              <Star className="h-6 w-6 mr-2" />
              Features
            </CardTitle>
          </CardHeader>
          <CardContent>
            <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
              <div>
                <h3 className="text-white font-semibold mb-2">Profile Management</h3>
                <ul className="text-slate-300 space-y-1 text-sm">
                  <li>• View profile information</li>
                  <li>• Edit personal details</li>
                  <li>• Change password</li>
                  <li>• Profile picture & cover photo</li>
                  <li>• Bio and social links</li>
                </ul>
              </div>
              <div>
                <h3 className="text-white font-semibold mb-2">Design Features</h3>
                <ul className="text-slate-300 space-y-1 text-sm">
                  <li>• Spider-Man themed dark design</li>
                  <li>• Red and blue color scheme</li>
                  <li>• Responsive layout</li>
                  <li>• Glass morphism effects</li>
                  <li>• Smooth animations</li>
                </ul>
              </div>
            </div>
          </CardContent>
        </Card>

        <div className="mt-12 text-center">
          <p className="text-slate-400 text-sm">
            Built with Next.js 15, TypeScript, Tailwind CSS, and shadcn/ui components
          </p>
        </div>
      </div>
    </div>
  );
}
