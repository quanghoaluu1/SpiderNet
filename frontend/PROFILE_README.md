# SpiderNet Profile System

A Spider-Man themed user profile system for your social media website, built with Next.js, TypeScript, Tailwind CSS, and shadcn/ui components.

## 🚀 Features

### Profile Management
- **View Profile**: Display user profile information with a beautiful Spider-Man themed interface
- **Edit Profile**: Update personal information including name, bio, location, website, etc.
- **Change Password**: Secure password change functionality
- **Profile Pictures**: Avatar and cover photo support
- **Social Links**: Website, phone number, and other contact information

### Design Features
- **Spider-Man Theme**: Dark theme with red and blue color scheme inspired by Spider-Man
- **Responsive Design**: Works perfectly on desktop, tablet, and mobile devices
- **Glass Morphism**: Modern glass effects with backdrop blur
- **Smooth Animations**: Loading states and hover effects
- **Professional Layout**: Clean, modern social media profile layout

## 📁 File Structure

```
src/
├── components/
│   └── UserProfile.tsx          # Main profile component
├── interfaces/
│   └── UserProfile.ts           # TypeScript interfaces
├── services/
│   └── api.ts                   # API service functions (updated)
├── app/
│   ├── profile/
│   │   ├── page.tsx            # Own profile page
│   │   └── [userId]/
│   │       └── page.tsx        # Other user's profile page
│   └── demo/
│       └── page.tsx            # Demo page with navigation
```

## 🛠️ API Integration

The profile system integrates with your existing backend API:

### Endpoints Used
- `GET /UserProfile/profile` - Get current user's profile
- `GET /UserProfile/{id}` - Get specific user's profile
- `PUT /UserProfile/profile` - Update profile
- `PUT /UserProfile/password` - Change password

### Authentication
- Uses JWT tokens from localStorage
- Automatically includes Bearer token in API requests
- Handles authentication errors gracefully

## 🎨 Components Used

### shadcn/ui Components
- Card, CardContent, CardHeader, CardTitle
- Avatar, AvatarFallback, AvatarImage
- Badge
- Button
- Input, Label, Textarea
- Dialog, DialogContent, DialogHeader, DialogTitle, DialogTrigger
- Select, SelectContent, SelectItem, SelectTrigger, SelectValue

### Icons
- Lucide React icons for a consistent, professional look

## 🔧 Usage

### View Own Profile
```tsx
import UserProfile from '@/components/UserProfile';

export default function MyProfilePage() {
  return <UserProfile />;
}
```

### View Another User's Profile
```tsx
import UserProfile from '@/components/UserProfile';

export default function UserProfilePage({ params }: { params: { userId: string } }) {
  return <UserProfile userId={params.userId} />;
}
```

## 🎯 Routes

- `/profile` - View your own profile
- `/profile/[userId]` - View another user's profile
- `/demo` - Demo page with navigation and features overview

## 🌟 Key Features

### Profile Information Display
- Full name and display name
- Username with @ symbol
- Email verification status
- Member since date
- Age calculation
- Bio/description
- Contact information (email, phone, website, location)
- Profile and cover photos

### Interactive Elements
- Edit profile modal with form validation
- Change password modal
- Profile picture upload (UI ready)
- Social action buttons (connect, message)
- Profile stats (views, connections, posts)
- Activity indicators

### Visual Design
- Spider-Man color scheme (red #c1121f, blue #0466c8)
- Dark theme with gradient backgrounds
- Glass morphism effects
- Responsive grid layouts
- Professional badges and status indicators
- Smooth hover animations

## 🔒 Security Features

- JWT token authentication
- Password change with confirmation
- Form validation
- Error handling for API failures
- Secure API request interceptors

## 📱 Responsive Design

The profile system is fully responsive and works great on:
- Desktop computers
- Tablets
- Mobile phones
- All modern browsers

## 🎨 Customization

The Spider-Man theme can be easily customized by modifying the color variables in `globals.css`:

```css
@theme {
  --color-spiderman-red: #c1121f;
  --color-spiderman-blue: #0466c8;
  --color-spiderman-dark-red: #780000;
  --color-spiderman-light-blue: #447BBE;
}
```

## 🚦 Getting Started

1. The profile system is ready to use with your existing authentication
2. Make sure your backend API is running and accessible
3. Set your API base URL in the environment variables
4. Navigate to `/profile` to view your profile
5. Use `/demo` to see all available features

Enjoy your new Spider-Man themed profile system! 🕷️🕸️
