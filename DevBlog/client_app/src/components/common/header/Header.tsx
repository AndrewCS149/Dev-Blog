import { Link } from 'react-router-dom';
import { Navbar } from '../../common';
import './Header.css';

const Header = () => {
    return (
        <div className="header">
            <div className="header-logo">
                <Link to="/">
                    <img src="Logo.png" alt="Picture of Logo" />
                </Link>
            </div>
            <div className="header-nav">
                <Navbar />
            </div>
            <div>
                Login SignUp
            </div>
        </div>
    )
}

export default Header;