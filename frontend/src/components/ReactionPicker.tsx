'use client';

import { useRef, useState, useEffect } from 'react';
import { ReactionType } from '@/interfaces/Post';
import { motion, AnimatePresence } from "framer-motion";
import { cn } from '@/lib/utils';

interface ReactionPickerProps {
  onReactionSelect: (type: ReactionType) => void;
  currentReaction?: ReactionType;
  className?: string;
}

const REACTIONS = [
  { type: ReactionType.Like, emoji: "👍", name: "Thích", color: "text-blue-500" },
  { type: ReactionType.Love, emoji: "❤️", name: "Yêu thích", color: "text-red-500" },
  { type: ReactionType.Haha, emoji: "😂", name: "Haha", color: "text-yellow-500" },
  { type: ReactionType.Wow, emoji: "😮", name: "Wow", color: "text-orange-500" },
  { type: ReactionType.Sad, emoji: "😢", name: "Buồn", color: "text-blue-400" },
  { type: ReactionType.Angry, emoji: "😠", name: "Phẫn nộ", color: "text-red-600" },
];

export default function ReactionPicker({
  onReactionSelect,
  currentReaction,
  className,
}: ReactionPickerProps) {
  const [isVisible, setIsVisible] = useState(false);
  const [hoveredReaction, setHoveredReaction] = useState<ReactionType | null>(null);
  const pressTimer = useRef<NodeJS.Timeout | null>(null);
  const hoverTimer = useRef<NodeJS.Timeout | null>(null);

  const handleReactionClick = (type: ReactionType) => {
    onReactionSelect(type);
    setIsVisible(false);
  };

  const getCurrentReactionData = () => {
    if (currentReaction !== undefined && currentReaction !== null) {
      return REACTIONS.find(r => r.type === currentReaction) ?? REACTIONS[0];
    }
    return REACTIONS[0]; // Default to Like
  };

  const currentReactionData = getCurrentReactionData();

  // Cleanup timers on unmount
  useEffect(() => {
    return () => {
      if (hoverTimer.current) {
        clearTimeout(hoverTimer.current);
      }
      if (pressTimer.current) {
        clearTimeout(pressTimer.current);
      }
    };
  }, []);

  const handleMouseEnter = () => {
    // Hiện picker sau khi hover 300ms
    hoverTimer.current = setTimeout(() => {
      setIsVisible(true);
    }, 300);
  };

  const handleMouseLeave = () => {
    // Hủy hover timer
    if (hoverTimer.current) {
      clearTimeout(hoverTimer.current);
      hoverTimer.current = null;
    }
    
    // Ẩn picker sau 200ms để cho phép di chuột vào picker
    hoverTimer.current = setTimeout(() => {
      setIsVisible(false);
    }, 200);
  };

  const handlePickerMouseEnter = () => {
    // Giữ picker hiển thị khi hover vào picker
    if (hoverTimer.current) {
      clearTimeout(hoverTimer.current);
      hoverTimer.current = null;
    }
  };

  const handlePickerMouseLeave = () => {
    // Ẩn picker khi rời khỏi picker
    setIsVisible(false);
  };

  const handleMouseDown = () => {
    pressTimer.current = setTimeout(() => setIsVisible(true), 500); // giữ chuột 0.5s -> mở popup
  };

  const handleMouseUp = () => {
    if (pressTimer.current) {
      clearTimeout(pressTimer.current);
    }
  };

  return (
    <div className={cn("relative inline-block", className)}>
      {/* Reaction Picker Popup */}
      <AnimatePresence>
        {isVisible && (
          <motion.div
            initial={{ opacity: 0, y: 20, scale: 0.95 }}
            animate={{ opacity: 1, y: 0, scale: 1 }}
            exit={{ opacity: 0, y: 20, scale: 0.95 }}
            transition={{ duration: 0.2 }}
            onMouseEnter={handlePickerMouseEnter}
            onMouseLeave={handlePickerMouseLeave}
            className="absolute bottom-full left-1/2 -translate-x-1/2 mb-2 flex items-center bg-white rounded-full px-3 py-2 shadow-xl border z-50"
          >
            {REACTIONS.map((reaction, index) => (
              <motion.button
                key={reaction.type}
                onClick={() => handleReactionClick(reaction.type)}
                onMouseEnter={() => setHoveredReaction(reaction.type)}
                onMouseLeave={() => setHoveredReaction(null)}
                whileHover={{ scale: 1.4, y: -8 }}
                transition={{ type: "spring", stiffness: 300, damping: 15, delay: index * 0.05 }}
                className="relative flex items-center justify-center w-10 h-10 mx-1 rounded-full"
              >
                <span className="text-2xl select-none">{reaction.emoji}</span>

                {/* Reaction tooltip */}
                {hoveredReaction === reaction.type && (
                  <div className="absolute -top-8 left-1/2 transform -translate-x-1/2 bg-gray-800 text-white text-xs px-2 py-1 rounded whitespace-nowrap z-10">
                    {reaction.name}
                  </div>
                )}
              </motion.button>
            ))}
          </motion.div>
        )}
      </AnimatePresence>

      {/* Main Reaction Button */}
      <button
        onMouseEnter={handleMouseEnter}
        onMouseLeave={handleMouseLeave}
        onMouseDown={handleMouseDown}
        onMouseUp={handleMouseUp}
        onClick={() => {
          if (!isVisible) {
            if (currentReaction !== undefined) {
              // Nếu đã có reaction -> toggle bỏ
              onReactionSelect(undefined as any);
            } else {
              // Nếu chưa có -> mặc định Like
              onReactionSelect(ReactionType.Like);
            }
          }
        }}
        className={cn(
          "flex items-center space-x-2 px-3 py-2 rounded-lg transition-all duration-200 hover:bg-white/10",
          currentReaction !== undefined && currentReactionData?.color
        )}
      >
        <span className="text-lg select-none">{currentReactionData?.emoji}</span>
        <span
          className={cn(
            "text-sm font-medium",
            currentReaction !== undefined ? currentReactionData?.color : "text-white/90"
          )}
        >
          {currentReaction !== undefined ? currentReactionData?.name : "Thích"}
        </span>
      </button>
    </div>
  );
}
