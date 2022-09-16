import { Link } from 'react-router-dom';
import './Navbar.css';

const Navbar = () => {
    return (
        <div className="navbar">
            <Link to="/">The Dev Blog</Link>
            <Link to="/posts">Posts</Link>
            <Link to="/about">About</Link>
        </div>
    );
};

export default Navbar;