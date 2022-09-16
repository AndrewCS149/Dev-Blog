import { Link } from 'react-router-dom';

const navbar = () => {
    return (
        <div>
            <li>
                <Link to="/">Home</Link>
            </li>
            <li>
                <Link to="/posts">Posts</Link>
            </li>
        </div>
    );
};

export default navbar;