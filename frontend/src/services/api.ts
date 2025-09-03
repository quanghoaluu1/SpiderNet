import api from "@/lib/axios";

export async function getWeather(){
    const res = await api.get("/WeatherForecast");
    return res.data;
}
export const authApi = {
    login: async (data: { email: string; password: string }) => {
        const res = await api.post("/Auth/login", data);
        return res.data;
    },
    register: async (data: { email: string; password: string; confirmPassword: string; firstName: string; lastName: string }) => {
        const res = await api.post("/Auth/register", data);
        return res.data;
    }
}