import { Navigate, Route, Routes } from "react-router-dom";
import Dashboard from "./pages/DashboardPage";
import RegisterPage from "./pages/RegisterPage";
import LoginPage from "./pages/LoginPage";
import ProtectedRoute from "./components/ProtectedRoute";
import BoardPage from "./pages/BoardPage";

function App() {
	return (
		<Routes>
			<Route path='/' element={<Navigate to='/dashboard' replace />} />
			<Route
				path='/dashboard'
				element={
					<ProtectedRoute>
						<Dashboard />
					</ProtectedRoute>
				}
			/>
			<Route
				path='/board/:boardId'
				element={
					<ProtectedRoute>
						<BoardPage />
					</ProtectedRoute>
				}
			/>
			<Route path='/register' element={<RegisterPage />} />
			<Route path='/login' element={<LoginPage />} />
			<Route path='*' element={<Navigate to='/dashboard' replace />} />
		</Routes>
	);
}

export default App;
