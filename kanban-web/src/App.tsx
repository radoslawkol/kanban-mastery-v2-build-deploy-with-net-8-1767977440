import { Navigate, Route, Routes } from "react-router-dom";
import HomePage from "./pages/HomePage";
import RegisterPage from "./pages/RegisterPage";
import LoginPage from "./pages/LoginPage";

function App() {
	return (
		<Routes>
			<Route path='/' element={<Navigate to='/dashboard' replace />} />
			<Route path='/dashboard' element={<HomePage />} />
			<Route path='/register' element={<RegisterPage />} />
			<Route path='/login' element={<LoginPage />} />
			<Route path='*' element={<Navigate to='/dashboard' replace />} />
		</Routes>
	);
}

export default App;
