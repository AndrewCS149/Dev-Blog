import { Link } from 'react-router-dom';
import './Navbar.css';

const Navbar = () => {
    return (
        <div className="navbar">
            <div>
                <Link className="hello" to="/">The Dev Blog</Link>
            </div>

            <div>
                <Link to="/posts">Posts</Link>
                <Link to="/about">About</Link>
            </div>

            <div>
                <Link to="/login">Login</Link>
                <Link to="/signup">SignUp</Link>
            </div>
        </div>
    );
};

export default Navbar;