import { TextField } from "@fluentui/react/lib/TextField";
import { FormEvent, useState } from "react";
import { useNavigate } from "react-router-dom";
import jwtDecode from "jwt-decode";
import IToken from "../interfaces/IToken";
import "./styles/SignIn.css";

const SignIn = () => {
    const [userName, setUsername] = useState("");
    const [password, setPassword] = useState("");
    const [error, setError] = useState<string>();
    const navigate = useNavigate();

    const handleSubmit = async (e: FormEvent<HTMLFormElement>) => {
        e.preventDefault();
        console.log(JSON.stringify({ userName, password }));

        fetch(`api/accounts/signin`, {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify({ userName, password })
        }).then(async (res) => {
            const data = await res.json();

            if (data.error) {
                setError(data.error);
                console.log(error);
            }

            const decodedToken: IToken = jwtDecode(data.token);
            localStorage.setItem("token", data.token)
            localStorage.setItem("userName", decodedToken.userName);
            localStorage.setItem("email", decodedToken.email);

            navigate("/");
            window.location.reload();
        });
    }

    return (
        <div className="sign-in-container">

            {error && <span>{error}</span>}

            <form onSubmit={handleSubmit} className="sign-in">
                <TextField
                    label="Username"
                    value={userName}
                    type="text"
                    onChange={(e) => setUsername(e.currentTarget.value)}
                    validateOnFocusOut
                    required
                />

                <TextField
                    label="Password"
                    value={password}
                    type="password"
                    onChange={(e) => setPassword(e.currentTarget.value)}
                    validateOnFocusOut
                    required
                />

                <button>Login</button>
            </form>
        </div>
    )
}

export default SignIn;