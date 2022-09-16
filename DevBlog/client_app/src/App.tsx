import { Header, About, Posts, Home } from './components/common';
import { BrowserRouter, Routes, Route } from 'react-router-dom';
import './App.css';

function App() {
    return (
        <BrowserRouter>
            <div>
                <Header />
                <Routes>
                    <Route path="/" element={<Home />} />
                    <Route path="/about" element={<About />} />
                    <Route path="/posts" element={<Posts />} />
                </Routes>
            </div>
        </BrowserRouter>
    );
}

export default App;
