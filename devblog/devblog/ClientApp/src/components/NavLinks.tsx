import { MdMenu } from "react-icons/md";
import { Link, useLocation } from "react-router-dom";
import SignOut from "../pages/SignOut";
import { Dispatch, SetStateAction, useEffect, useState } from "react";
import { GetIsAdmin } from "./AuthenticationService";

interface IProps {
    setIsBellClicked: Dispatch<SetStateAction<boolean>>,
    setBellDisplay: Dispatch<SetStateAction<string>>,
    setIsMenuClicked: Dispatch<SetStateAction<boolean>>,
    handleMenuClick: () => void,
    isMenuClicked: boolean,
    isBellClicked: boolean,
    loggedIn: boolean,
}

const NavLinks = (props: IProps) => {
    const [isAdmin, setIsAdmin] = useState(false);
    const [navDisplay, setNavDisplay] = useState("none")
    const location = useLocation();

    const isActive = (path: string) => {
        return location.pathname === path ? "active" : "non-active";
    };

    useEffect(() => {
        if (props.isMenuClicked) {
            setNavDisplay("flex")
        }
        else {
            setNavDisplay("none")
        }
    }, [props.isMenuClicked])

    useEffect(() => {
        setIsAdmin(GetIsAdmin)
    }, []);

    return (
        <div className="nav-drop-down">
            <MdMenu className="nav-icon" onClick={props.handleMenuClick} />

            <div className="nav-links" style={{ display: navDisplay }}>
                <Link to="/" className={isActive("/")}>Home</Link>
                <Link to="/posts" className={isActive("/posts")}>Posts</Link>
                <Link to="/about" className={isActive("/about")}>About</Link>
                {isAdmin && <Link to="/insights" className={isActive("/insights")}>Insights</Link>}

                {props.loggedIn && <SignOut />}

                {!props.loggedIn &&
                    <span className="nav-accounts">
                        <Link to="/signin" className={isActive("/signin")}>Login</Link>
                        <Link to="/signup" className={isActive("/signup")}>SignUp</Link>
                    </span>
                }
            </div>
        </div>
    )
}

export default NavLinks;