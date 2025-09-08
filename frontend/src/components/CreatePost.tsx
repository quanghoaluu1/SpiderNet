'use client';

import { useState, useRef } from 'react';
import Image from 'next/image';
import { Video, Plus, X, Globe, Users, Lock, ImageIcon } from 'lucide-react';
import { User } from '@/types';
import { CreatePostRequest, PostPrivacy } from '@/interfaces/Post';
import { postApi } from '@/services/api';
import VideoPlayer from './VideoPlayer';
import { Button } from '@/components/ui/button';
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from '@/components/ui/select';
import { toast } from 'sonner';

interface CreatePostProps {
  user: User;
  onPostCreated?: () => void;
}

export default function CreatePost({ user, onPostCreated }: CreatePostProps) {
  const [content, setContent] = useState('');
  const [privacy, setPrivacy] = useState<PostPrivacy>(PostPrivacy.Public);
  const [selectedImage, setSelectedImage] = useState<File | null>(null);
  const [selectedVideo, setSelectedVideo] = useState<File | null>(null);
  const [imagePreview, setImagePreview] = useState<string>('');
  const [videoPreview, setVideoPreview] = useState<string>('');
  const [isLoading, setIsLoading] = useState(false);
  const [isExpanded, setIsExpanded] = useState(false);
  
  const imageInputRef = useRef<HTMLInputElement>(null);
  const videoInputRef = useRef<HTMLInputElement>(null);

  const handleImageSelect = (e: React.ChangeEvent<HTMLInputElement>) => {
    const file = e.target.files?.[0];
    if (file) {
      if (file.size > 10 * 1024 * 1024) { // 10MB
        toast.error('Image size must be less than 10MB');
        return;
      }
      setSelectedImage(file);
      setSelectedVideo(null);
      setVideoPreview('');
      
      const reader = new FileReader();
      reader.onload = (e) => setImagePreview(e.target?.result as string);
      reader.readAsDataURL(file);
      setIsExpanded(true);
    }
  };

  const handleVideoSelect = (e: React.ChangeEvent<HTMLInputElement>) => {
    const file = e.target.files?.[0];
    if (file) {
      if (file.size > 100 * 1024 * 1024) { // 100MB
        toast.error('Video size must be less than 100MB');
        return;
      }
      setSelectedVideo(file);
      setSelectedImage(null);
      setImagePreview('');
      
      const reader = new FileReader();
      reader.onload = (e) => setVideoPreview(e.target?.result as string);
      reader.readAsDataURL(file);
      setIsExpanded(true);
    }
  };

  const removeMedia = () => {
    setSelectedImage(null);
    setSelectedVideo(null);
    setImagePreview('');
    setVideoPreview('');
    if (imageInputRef.current) imageInputRef.current.value = '';
    if (videoInputRef.current) videoInputRef.current.value = '';
  };

  const handleSubmit = async () => {
    if (!content.trim() && !selectedImage && !selectedVideo) {
      toast.error('Please add some content or media');
      return;
    }

    setIsLoading(true);
    try {
      const postData: CreatePostRequest = {
        content: content.trim(),
        privacy,
        image: selectedImage || undefined,
        video: selectedVideo || undefined,
      };

      await postApi.createPost(postData);
      
      // Reset form
      setContent('');
      setPrivacy(PostPrivacy.Public);
      removeMedia();
      setIsExpanded(false);
      
      toast.success('Post created successfully!');
      onPostCreated?.();
    } catch (error) {
      console.error('Error creating post:', error);
      toast.error('Failed to create post');
    } finally {
      setIsLoading(false);
    }
  };

  const getPrivacyIcon = () => {
    switch (privacy) {
      case PostPrivacy.Public:
        return <Globe className="w-4 h-4" />;
      case PostPrivacy.Friends:
        return <Users className="w-4 h-4" />;
      case PostPrivacy.Private:
        return <Lock className="w-4 h-4" />;
      default:
        return <Globe className="w-4 h-4" />;
    }
  };

  const getPrivacyLabel = () => {
    switch (privacy) {
      case PostPrivacy.Public:
        return 'Public';
      case PostPrivacy.Friends:
        return 'Friends';
      case PostPrivacy.Private:
        return 'Only me';
      default:
        return 'Public';
    }
  };

  return (
    <div className="glass-morphism rounded-lg shadow-xl p-4 mb-6 border border-white/10">
      {/* Header */}
      <div className="flex items-center space-x-3">
        <div className="bg-spiderman-red p-1 rounded-full">
          <Image 
            src={user.avatarUrl || "/icons/icon.png"} 
            alt={user.firstName} 
            width={36} 
            height={36} 
            className="rounded-full object-cover" 
          />
        </div>
        <div className="flex-1">
          <textarea
            value={content}
            onChange={(e) => setContent(e.target.value)}
            onFocus={() => setIsExpanded(true)}
            placeholder={`What's on your mind, ${user.firstName}?`}
            className="w-full bg-transparent resize-none outline-none text-white placeholder-white/60 text-lg"
            rows={isExpanded ? 3 : 1}
          />
        </div>
      </div>

      {/* Media Preview */}
      {(imagePreview || videoPreview) && (
        <div className="mt-4 relative">
          <button
            onClick={removeMedia}
            className="absolute top-2 right-2 z-10 bg-black/50 rounded-full p-1 text-white hover:bg-black/70 transition-colors"
          >
            <X className="w-4 h-4" />
          </button>
          
          {imagePreview && (
            <div className="rounded-lg overflow-hidden">
              <Image
                src={imagePreview}
                alt="Preview"
                width={500}
                height={300}
                className="w-full h-auto max-h-96 object-cover"
              />
            </div>
          )}
          
          {videoPreview && (
            <VideoPlayer
              src={videoPreview}
              className="max-h-96"
              controls={true}
              fluid={true}
            />
          )}
        </div>
      )}

      {/* Actions */}
      {isExpanded && (
        <div className="mt-4 pt-3 border-t border-white/20">
          <div className="flex justify-between items-center">
            <div className="flex items-center space-x-4">
              {/* Privacy Selector */}
              <Select value={privacy.toString()} onValueChange={(value) => setPrivacy(parseInt(value) as PostPrivacy)}>
                <SelectTrigger className="w-32 bg-white/10 border-white/20 text-white">
                  <div className="flex items-center space-x-2">
                    {getPrivacyIcon()}
                    <SelectValue />
                  </div>
                </SelectTrigger>
                <SelectContent className="bg-black/90 border-white/20">
                  <SelectItem value="0" className="text-white hover:bg-white/10">
                    <div className="flex items-center space-x-2">
                      <Globe className="w-4 h-4" />
                      <span>Public</span>
                    </div>
                  </SelectItem>
                  <SelectItem value="1" className="text-white hover:bg-white/10">
                    <div className="flex items-center space-x-2">
                      <Users className="w-4 h-4" />
                      <span>Friends</span>
                    </div>
                  </SelectItem>
                  <SelectItem value="2" className="text-white hover:bg-white/10">
                    <div className="flex items-center space-x-2">
                      <Lock className="w-4 h-4" />
                      <span>Only me</span>
                    </div>
                  </SelectItem>
                </SelectContent>
              </Select>

              {/* Media Buttons */}
              <div className="flex space-x-2">
                <button
                  onClick={() => imageInputRef.current?.click()}
                  className="flex items-center space-x-2 text-white/70 hover:text-spiderman-red transition-colors p-2 rounded-lg hover:bg-white/10"
                >
                  <ImageIcon className="w-5 h-5" />
                  <span className="text-sm">Photo</span>
                </button>
                
                <button
                  onClick={() => videoInputRef.current?.click()}
                  className="flex items-center space-x-2 text-white/70 hover:text-spiderman-blue transition-colors p-2 rounded-lg hover:bg-white/10"
                >
                  <Video className="w-5 h-5" />
                  <span className="text-sm">Video</span>
                </button>
              </div>
            </div>

            <Button
              onClick={handleSubmit}
              disabled={isLoading || (!content.trim() && !selectedImage && !selectedVideo)}
              className="bg-spiderman-red hover:bg-spiderman-dark-red text-white px-6 py-2 rounded-lg font-medium transition-colors shadow-lg disabled:opacity-50"
            >
              {isLoading ? 'Posting...' : 'Post'}
            </Button>
          </div>
        </div>
      )}

      {/* Simple Actions (when not expanded) */}
      {!isExpanded && (
        <div className="flex justify-between items-center mt-3 pt-3 border-t border-white/20">
          <div className="flex space-x-4">
            <button
              onClick={() => videoInputRef.current?.click()}
              className="flex items-center space-x-2 text-white/70 hover:text-spiderman-blue transition-colors"
            >
              <Video className="w-5 h-5" />
              <span className="text-sm">Live Video</span>
            </button>
            <button
              onClick={() => imageInputRef.current?.click()}
              className="flex items-center space-x-2 text-white/70 hover:text-spiderman-red transition-colors"
            >
              <Plus className="w-5 h-5" />
              <span className="text-sm">Photo/Video</span>
            </button>
          </div>
          <button
            onClick={() => setIsExpanded(true)}
            className="bg-spiderman-red text-white px-4 py-2 rounded-lg text-sm font-medium hover:bg-spiderman-dark-red transition-colors shadow-lg"
          >
            Post
          </button>
        </div>
      )}

      {/* Hidden File Inputs */}
      <input
        ref={imageInputRef}
        type="file"
        accept="image/*"
        onChange={handleImageSelect}
        className="hidden"
      />
      <input
        ref={videoInputRef}
        type="file"
        accept="video/*"
        onChange={handleVideoSelect}
        className="hidden"
      />
    </div>
  );
}
