"use client";

import React, { useState, useEffect } from 'react';
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
import { Avatar, AvatarFallback, AvatarImage } from '@/components/ui/avatar';
import { Badge } from '@/components/ui/badge';
import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';
import { Label } from '@/components/ui/label';
import { Textarea } from '@/components/ui/textarea';
import { Dialog, DialogContent, DialogHeader, DialogTitle, DialogTrigger } from '@/components/ui/dialog';
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from '@/components/ui/select';
import { 
  User, 
  Mail, 
  Calendar, 
  MapPin, 
  Globe, 
  Phone, 
  Edit3, 
  Camera,
  Shield,
  Lock,
  CheckCircle,
  XCircle
} from 'lucide-react';
import { UserProfileDto, UpdateProfileRequest, ChangePasswordRequest } from '@/interfaces/UserProfile';
import { userProfileApi } from '@/services/api';

interface UserProfileProps {
  userId?: string; // If provided, view another user's profile, otherwise view own profile
}

export default function UserProfile({ userId }: UserProfileProps) {
  const [profile, setProfile] = useState<UserProfileDto | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [isEditDialogOpen, setIsEditDialogOpen] = useState(false);
  const [isPasswordDialogOpen, setIsPasswordDialogOpen] = useState(false);
  const [editForm, setEditForm] = useState<UpdateProfileRequest>({});
  const [passwordForm, setPasswordForm] = useState<ChangePasswordRequest>({
    currentPassword: '',
    newPassword: '',
    confirmNewPassword: ''
  });

  useEffect(() => {
    fetchProfile();
  }, [userId]);

  const fetchProfile = async () => {
    try {
      setLoading(true);
      const data = userId 
        ? await userProfileApi.getUserProfile(userId)
        : await userProfileApi.getMyProfile();
      setProfile(data);
      setEditForm({
        firstName: data.firstName,
        lastName: data.lastName,
        username: data.username,
        bio: data.bio || '',
        dateOfBirth: data.dateOfBirth,
        location: data.location || '',
        website: data.website || '',
        phoneNumber: data.phoneNumber || '',
        gender: data.gender || ''
      });
    } catch (err) {
      setError('Failed to load profile');
      console.error('Error fetching profile:', err);
    } finally {
      setLoading(false);
    }
  };

  const handleUpdateProfile = async (e: React.FormEvent) => {
    e.preventDefault();
    try {
      const updatedProfile = await userProfileApi.updateProfile(editForm);
      setProfile(updatedProfile);
      setIsEditDialogOpen(false);
    } catch (err) {
      console.error('Error updating profile:', err);
    }
  };

  const handleChangePassword = async (e: React.FormEvent) => {
    e.preventDefault();
    if (passwordForm.newPassword !== passwordForm.confirmNewPassword) {
      alert('New passwords do not match');
      return;
    }
    try {
      await userProfileApi.changePassword(passwordForm);
      setIsPasswordDialogOpen(false);
      setPasswordForm({ currentPassword: '', newPassword: '', confirmNewPassword: '' });
      alert('Password changed successfully');
    } catch (err) {
      console.error('Error changing password:', err);
    }
  };

  if (loading) {
    return (
      <div className="flex items-center justify-center min-h-screen bg-gradient-to-br from-slate-900 via-red-950 to-blue-950">
        <div className="animate-spin rounded-full h-32 w-32 border-b-2 border-red-500"></div>
      </div>
    );
  }

  if (error || !profile) {
    return (
      <div className="flex items-center justify-center min-h-screen bg-gradient-to-br from-slate-900 via-red-950 to-blue-950">
        <Card className="bg-slate-800 border-red-500/20">
          <CardContent className="p-6 text-center">
            <XCircle className="h-16 w-16 text-red-500 mx-auto mb-4" />
            <p className="text-red-400">{error || 'Profile not found'}</p>
          </CardContent>
        </Card>
      </div>
    );
  }

  return (
    <div className="min-h-screen bg-slate-900">
      {/* Cover Photo Section */}
      <div className="relative bg-slate-800">
        <div 
          className="h-80 w-2/3 mx-auto bg-gradient-to-r from-red-600 via-red-800 to-blue-800 relative overflow-hidden rounded-lg"
          style={{
            backgroundImage: profile.coverPhotoUrl ? `url(${profile.coverPhotoUrl})` : undefined,
            backgroundSize: 'cover',
            backgroundPosition: 'center'
          }}
        >
          <div className="absolute inset-0 bg-gradient-to-r from-red-900/50 to-blue-900/50"></div>
          <div className="absolute inset-0 opacity-30" style={{
            backgroundImage: `url("data:image/svg+xml,%3Csvg width='60' height='60' viewBox='0 0 60 60' xmlns='http://www.w3.org/2000/svg'%3E%3Cg fill='none' fill-rule='evenodd'%3E%3Cg fill='%23ffffff' fill-opacity='0.05'%3E%3Ccircle cx='30' cy='30' r='2'/%3E%3C/g%3E%3C/g%3E%3C/svg%3E")`
          }}></div>
          
          {/* Cover Photo Edit Button */}
          {profile.isOwnProfile && (
            <div className="absolute bottom-4 right-4">
              <Button size="sm" className="bg-slate-800/80 hover:bg-slate-700/80 text-white border-slate-600">
                <Camera className="h-4 w-4 mr-2" />
                Edit Cover Photo
              </Button>
            </div>
          )}
        </div>
        
        {/* Profile Header */}
        <div className="bg-slate-800 border-b border-slate-700">
          <div className="max-w-7xl mx-auto px-4">
            <div className="flex flex-col md:flex-row md:items-end md:justify-between py-4">
              {/* Avatar and Name */}
              <div className="flex flex-col md:flex-row md:items-end space-y-4 md:space-y-0 md:space-x-6">
                <div className="relative -mt-20 md:-mt-16">
                  <Avatar className="h-40 w-40 border-4 border-slate-800 shadow-2xl">
                    <AvatarImage src={profile.avatarUrl} alt={profile.displayName} />
                    <AvatarFallback className="bg-gradient-to-br from-red-600 to-blue-600 text-white text-3xl font-bold">
                      {profile.firstName[0]}{profile.lastName[0]}
                    </AvatarFallback>
                  </Avatar>
                  {profile.isOwnProfile && (
                    <Button 
                      size="sm" 
                      className="absolute bottom-2 right-2 rounded-full h-10 w-10 p-0 bg-slate-700 hover:bg-slate-600 border-2 border-slate-800"
                    >
                      <Camera className="h-5 w-5" />
                    </Button>
                  )}
                </div>
                
                <div className="pb-4">
                  <h1 className="text-4xl font-bold text-white flex items-center">
                    {profile.displayName}
                    {profile.isEmailConfirmed && (
                      <CheckCircle className="h-6 w-6 text-blue-500 ml-2" />
                    )}
                  </h1>
                  <p className="text-slate-400 text-lg">@{profile.username}</p>
                  <p className="text-slate-500 mt-1">567 friends</p>
                </div>
              </div>
              
              {/* Action Buttons */}
              <div className="flex space-x-3 pb-4">
                {profile.isOwnProfile ? (
                  <>
                    <Dialog open={isEditDialogOpen} onOpenChange={setIsEditDialogOpen}>
                      <DialogTrigger asChild>
                        <Button className="bg-red-600 hover:bg-red-700 text-white px-6">
                          <Edit3 className="h-4 w-4 mr-2" />
                          Edit Profile
                        </Button>
                      </DialogTrigger>
                      <DialogContent className="bg-slate-800 border-red-500/20 text-white max-w-2xl max-h-[90vh] overflow-y-auto">
                        <DialogHeader>
                          <DialogTitle className="text-red-400">Edit Profile</DialogTitle>
                        </DialogHeader>
                        <form onSubmit={handleUpdateProfile} className="space-y-4">
                          <div className="grid grid-cols-2 gap-4">
                            <div>
                              <Label htmlFor="firstName" className="text-slate-300">First Name</Label>
                              <Input
                                id="firstName"
                                value={editForm.firstName || ''}
                                onChange={(e) => setEditForm({ ...editForm, firstName: e.target.value })}
                                className="bg-slate-700 border-slate-600 text-white"
                              />
                            </div>
                            <div>
                              <Label htmlFor="lastName" className="text-slate-300">Last Name</Label>
                              <Input
                                id="lastName"
                                value={editForm.lastName || ''}
                                onChange={(e) => setEditForm({ ...editForm, lastName: e.target.value })}
                                className="bg-slate-700 border-slate-600 text-white"
                              />
                            </div>
                          </div>
                          
                          <div>
                            <Label htmlFor="username" className="text-slate-300">Username</Label>
                            <Input
                              id="username"
                              value={editForm.username || ''}
                              onChange={(e) => setEditForm({ ...editForm, username: e.target.value })}
                              className="bg-slate-700 border-slate-600 text-white"
                            />
                          </div>

                          <div>
                            <Label htmlFor="bio" className="text-slate-300">Bio</Label>
                            <Textarea
                              id="bio"
                              value={editForm.bio || ''}
                              onChange={(e) => setEditForm({ ...editForm, bio: e.target.value })}
                              className="bg-slate-700 border-slate-600 text-white"
                              rows={3}
                            />
                          </div>

                          <div className="grid grid-cols-2 gap-4">
                            <div>
                              <Label htmlFor="location" className="text-slate-300">Location</Label>
                              <Input
                                id="location"
                                value={editForm.location || ''}
                                onChange={(e) => setEditForm({ ...editForm, location: e.target.value })}
                                className="bg-slate-700 border-slate-600 text-white"
                              />
                            </div>
                            <div>
                              <Label htmlFor="website" className="text-slate-300">Website</Label>
                              <Input
                                id="website"
                                value={editForm.website || ''}
                                onChange={(e) => setEditForm({ ...editForm, website: e.target.value })}
                                className="bg-slate-700 border-slate-600 text-white"
                              />
                            </div>
                          </div>

                          <div className="grid grid-cols-2 gap-4">
                            <div>
                              <Label htmlFor="phoneNumber" className="text-slate-300">Phone Number</Label>
                              <Input
                                id="phoneNumber"
                                value={editForm.phoneNumber || ''}
                                onChange={(e) => setEditForm({ ...editForm, phoneNumber: e.target.value })}
                                className="bg-slate-700 border-slate-600 text-white"
                              />
                            </div>
                            <div>
                              <Label htmlFor="gender" className="text-slate-300">Gender</Label>
                              <Select value={editForm.gender || ''} onValueChange={(value) => setEditForm({ ...editForm, gender: value })}>
                                <SelectTrigger className="bg-slate-700 border-slate-600 text-white">
                                  <SelectValue placeholder="Select gender" />
                                </SelectTrigger>
                                <SelectContent className="bg-slate-700 border-slate-600">
                                  <SelectItem value="male">Male</SelectItem>
                                  <SelectItem value="female">Female</SelectItem>
                                  <SelectItem value="other">Other</SelectItem>
                                  <SelectItem value="prefer-not-to-say">Prefer not to say</SelectItem>
                                </SelectContent>
                              </Select>
                            </div>
                          </div>

                          <div>
                            <Label htmlFor="dateOfBirth" className="text-slate-300">Date of Birth</Label>
                            <Input
                              id="dateOfBirth"
                              type="date"
                              value={editForm.dateOfBirth ? editForm.dateOfBirth.split('T')[0] : ''}
                              onChange={(e) => setEditForm({ ...editForm, dateOfBirth: e.target.value })}
                              className="bg-slate-700 border-slate-600 text-white"
                            />
                          </div>

                          <div className="flex justify-end space-x-2 pt-4">
                            <Button type="button" variant="outline" onClick={() => setIsEditDialogOpen(false)}>
                              Cancel
                            </Button>
                            <Button type="submit" className="bg-red-600 hover:bg-red-700">
                              Save Changes
                            </Button>
                          </div>
                        </form>
                      </DialogContent>
                    </Dialog>
                    
                    <Dialog open={isPasswordDialogOpen} onOpenChange={setIsPasswordDialogOpen}>
                      <DialogTrigger asChild>
                        <Button variant="outline" className="border-slate-600 hover:bg-slate-700 text-white px-6">
                          <Lock className="h-4 w-4 mr-2" />
                          Settings
                        </Button>
                      </DialogTrigger>
                      <DialogContent className="bg-slate-800 border-red-500/20 text-white">
                        <DialogHeader>
                          <DialogTitle className="text-red-400">Change Password</DialogTitle>
                        </DialogHeader>
                        <form onSubmit={handleChangePassword} className="space-y-4">
                          <div>
                            <Label htmlFor="currentPassword" className="text-slate-300">Current Password</Label>
                            <Input
                              id="currentPassword"
                              type="password"
                              value={passwordForm.currentPassword}
                              onChange={(e) => setPasswordForm({ ...passwordForm, currentPassword: e.target.value })}
                              className="bg-slate-700 border-slate-600 text-white"
                              required
                            />
                          </div>
                          <div>
                            <Label htmlFor="newPassword" className="text-slate-300">New Password</Label>
                            <Input
                              id="newPassword"
                              type="password"
                              value={passwordForm.newPassword}
                              onChange={(e) => setPasswordForm({ ...passwordForm, newPassword: e.target.value })}
                              className="bg-slate-700 border-slate-600 text-white"
                              required
                            />
                          </div>
                          <div>
                            <Label htmlFor="confirmNewPassword" className="text-slate-300">Confirm New Password</Label>
                            <Input
                              id="confirmNewPassword"
                              type="password"
                              value={passwordForm.confirmNewPassword}
                              onChange={(e) => setPasswordForm({ ...passwordForm, confirmNewPassword: e.target.value })}
                              className="bg-slate-700 border-slate-600 text-white"
                              required
                            />
                          </div>
                          <div className="flex justify-end space-x-2 pt-4">
                            <Button type="button" variant="outline" onClick={() => setIsPasswordDialogOpen(false)}>
                              Cancel
                            </Button>
                            <Button type="submit" className="bg-red-600 hover:bg-red-700">
                              Change Password
                            </Button>
                          </div>
                        </form>
                      </DialogContent>
                    </Dialog>
                  </>
                ) : (
                  <>
                    <Button className="bg-red-600 hover:bg-red-700 text-white px-8">
                      Add Friend
                    </Button>
                    <Button variant="outline" className="border-slate-600 hover:bg-slate-700 text-white px-8">
                      Message
                    </Button>
                  </>
                )}
              </div>
            </div>
          </div>
        </div>
      </div>
      
      {/* Content Area */}
      <div className="max-w-7xl mx-auto px-4 py-6">
        <div className="grid grid-cols-1 lg:grid-cols-3 gap-6">
          {/* Left Sidebar - About */}
          <div className="lg:col-span-1 space-y-6">
            <Card className="bg-slate-800 border-slate-700">
              <CardHeader>
                <CardTitle className="text-white text-lg">About</CardTitle>
              </CardHeader>
              <CardContent className="space-y-4">
                {profile.bio && (
                  <p className="text-slate-300">{profile.bio}</p>
                )}
                
                <div className="space-y-3">
                  <div className="flex items-center space-x-3 text-slate-300">
                    <Mail className="h-5 w-5 text-red-400" />
                    <span>{profile.email}</span>
                  </div>
                  
                  {profile.location && (
                    <div className="flex items-center space-x-3 text-slate-300">
                      <MapPin className="h-5 w-5 text-red-400" />
                      <span>Lives in {profile.location}</span>
                    </div>
                  )}
                  
                  {profile.website && (
                    <div className="flex items-center space-x-3">
                      <Globe className="h-5 w-5 text-red-400" />
                      <a 
                        href={profile.website} 
                        target="_blank" 
                        rel="noopener noreferrer"
                        className="text-blue-400 hover:text-blue-300 transition-colors"
                      >
                        {profile.website}
                      </a>
                    </div>
                  )}
                  
                  <div className="flex items-center space-x-3 text-slate-300">
                    <Calendar className="h-5 w-5 text-red-400" />
                    <span>Joined {new Date(profile.createdAt).toLocaleDateString('en-US', { month: 'long', year: 'numeric' })}</span>
                  </div>
                  
                  <div className="flex items-center space-x-3 text-slate-300">
                    <Shield className="h-5 w-5 text-red-400" />
                    <span>{profile.memberSince}</span>
                  </div>
                </div>
              </CardContent>
            </Card>
            
            {/* Friends Preview */}
            <Card className="bg-slate-800 border-slate-700">
              <CardHeader>
                <CardTitle className="text-white text-lg flex items-center justify-between">
                  Friends
                  <span className="text-sm text-slate-400">567</span>
                </CardTitle>
              </CardHeader>
              <CardContent>
                <div className="grid grid-cols-3 gap-2">
                  {[1,2,3,4,5,6].map((i) => (
                    <div key={i} className="aspect-square bg-gradient-to-br from-red-600/20 to-blue-600/20 rounded-lg border border-slate-600"></div>
                  ))}
                </div>
                <Button variant="outline" className="w-full mt-4 border-slate-600 hover:bg-slate-700 text-slate-300">
                  See all friends
                </Button>
              </CardContent>
            </Card>
          </div>
          
          {/* Right Content - Posts/Timeline */}
          <div className="lg:col-span-2 space-y-6">
            {/* Create Post (if own profile) */}
            {profile.isOwnProfile && (
              <Card className="bg-slate-800 border-slate-700">
                <CardContent className="p-4">
                  <div className="flex space-x-3">
                    <Avatar className="h-10 w-10">
                      <AvatarImage src={profile.avatarUrl} alt={profile.displayName} />
                      <AvatarFallback className="bg-gradient-to-br from-red-600 to-blue-600 text-white">
                        {profile.firstName[0]}{profile.lastName[0]}
                      </AvatarFallback>
                    </Avatar>
                    <div className="flex-1">
                      <Button 
                        variant="outline" 
                        className="w-full justify-start bg-slate-700 border-slate-600 hover:bg-slate-500 text-slate-400 h-12"
                      >
                        What&apos;s on your mind, {profile.firstName}?
                      </Button>
                    </div>
                  </div>
                </CardContent>
              </Card>
            )}
            
            {/* Posts */}
            <Card className="bg-slate-800 border-slate-700">
              <CardContent className="p-6">
                <div className="text-center text-slate-400 py-12">
                  <User className="h-16 w-16 mx-auto mb-4 opacity-50" />
                  <h3 className="text-xl font-semibold mb-2">No posts yet</h3>
                  <p>When {profile.isOwnProfile ? 'you' : profile.firstName} posts something, it will appear here.</p>
                </div>
              </CardContent>
            </Card>
          </div>
        </div>
      </div>
    </div>
  );
}
