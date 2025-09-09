'use client';

import { useEffect, useRef } from 'react';
import videojs from 'video.js';
import 'video.js/dist/video-js.css';

interface VideoPlayerProps {
  src: string;
  poster?: string;
  className?: string;
  autoplay?: boolean;
  controls?: boolean;
  fluid?: boolean;
  responsive?: boolean;
}

export default function VideoPlayer({ 
  src, 
  poster, 
  className = "",
  autoplay = false,
  controls = true,
  fluid = true,
  responsive = true
}: VideoPlayerProps) {
  const videoRef = useRef<HTMLVideoElement>(null);
  const playerRef = useRef<ReturnType<typeof videojs> | null>(null);

  useEffect(() => {
    console.log('VideoPlayer: src =', src);
    console.log('VideoPlayer: videoRef.current =', videoRef.current);
    
    if (!videoRef.current) return;

    // Initialize Video.js player
    const player = videojs(videoRef.current, {
      controls,
      autoplay,
      preload: 'metadata',
      fluid,
      responsive,
      poster,
      sources: [{
        src,
        type: getVideoType(src)
      }],
      playbackRates: [0.5, 1, 1.25, 1.5, 2],
      plugins: {},
    });

    // Add error handling
    player.on('error', function() {
      const error = player.error();
      console.error('VideoPlayer error:', error);
    });

    player.on('loadstart', function() {
      console.log('VideoPlayer: Loading started');
    });

    player.on('canplay', function() {
      console.log('VideoPlayer: Can play');
    });

    playerRef.current = player;

    // Custom styling for SpiderNet theme
    player.ready(() => {
      const playerElement = player.el();
      if (playerElement) {
        playerElement.classList.add('spidernet-video-player');
      }
    });

    return () => {
      if (playerRef.current) {
        playerRef.current.dispose();
        playerRef.current = null;
      }
    };
  }, [src, poster, autoplay, controls, fluid, responsive]);

  const getVideoType = (url: string): string => {
    const extension = url.split('.').pop()?.toLowerCase();
    switch (extension) {
      case 'mp4':
        return 'video/mp4';
      case 'webm':
        return 'video/webm';
      case 'ogg':
        return 'video/ogg';
      case 'avi':
        return 'video/avi';
      case 'mov':
        return 'video/mov';
      default:
        return 'video/mp4';
    }
  };

  return (
    <div className={`video-player-container ${className}`}>
      <video
        ref={videoRef}
        className="video-js vjs-default-skin"
        data-setup="{}"
        playsInline
      />
      
      <style jsx global>{`
        .spidernet-video-player .vjs-control-bar {
          background: rgba(0, 0, 0, 0.8);
          backdrop-filter: blur(10px);
        }
        
        .spidernet-video-player .vjs-big-play-button {
          background: rgba(220, 38, 127, 0.9);
          border: none;
          border-radius: 50%;
          width: 80px;
          height: 80px;
          line-height: 80px;
          margin-top: -40px;
          margin-left: -40px;
        }
        
        .spidernet-video-player .vjs-big-play-button:hover {
          background: rgba(220, 38, 127, 1);
        }
        
        .spidernet-video-player .vjs-play-progress {
          background: linear-gradient(90deg, #dc267f, #648fff);
        }
        
        .spidernet-video-player .vjs-volume-level {
          background: linear-gradient(90deg, #dc267f, #648fff);
        }
        
        .video-player-container {
          border-radius: 12px;
          overflow: hidden;
          background: #000;
        }
        
        .video-player-container .video-js {
          border-radius: 12px;
        }
      `}</style>
    </div>
  );
}
