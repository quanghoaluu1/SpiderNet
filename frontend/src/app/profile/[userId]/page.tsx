import UserProfile from '@/components/UserProfile';

interface ProfilePageProps {
  params: {
    userId: string;
  };
}

export default function UserProfilePage({ params }: ProfilePageProps) {
  return <UserProfile userId={params.userId} />;
}
