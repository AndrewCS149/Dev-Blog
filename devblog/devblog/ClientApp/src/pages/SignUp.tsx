import { TextField } from "@fluentui/react/lib/TextField";
import { FormEvent, useState } from "react";
import { Hash, useNavigate } from "react-router-dom";
import IToken from "../interfaces/IToken";
import jwtDecode from "jwt-decode";
import "./styles/SignUp.css";

const SignUp = () => {
    const [userName, setUsername] = useState("");
    const [email, setEmail] = useState("");
    const [passwordHash, setPasswordHash] = useState<Hash>("");
    const [confirmPasswordHash, setConfirmPasswordHash] = useState<Hash>("");
    const [errors, setErrors] = useState<string[]>();
    const navigate = useNavigate();

    const handleSubmit = async (e: FormEvent<HTMLFormElement>) => {
        e.preventDefault();

        // ensure password & confirm password matches
        if (passwordHash !== confirmPasswordHash) {
            setErrors(["Passwords do not match"]);
            return;
        }

        console.log(`${userName} || ${email}`)
        await fetch("api/accounts/signup", {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify({ userName, email, passwordHash })
        }).then(async res => {
            const data = await res.json();

            // if there are any errors, store them in the 'errors' array for display
            if (data.errors) {
                const tmpErrors: string[] = [];
                for (let i = 0; i < data.errors.length; i++) {
                    const key = Object.keys(data.errors)[i] as keyof typeof data.errors;
                    tmpErrors.push(data.errors[key]["description"]);
                }
                setErrors(tmpErrors);
            }

            const decodedToken: IToken = jwtDecode(data.token);
            localStorage.setItem("token", data.token)
            localStorage.setItem("userName", decodedToken.userName);
            localStorage.setItem("email", decodedToken.email);

            navigate("/");
            window.location.reload();
        }).catch(e => {
            console.log("Error Creating Account: ", e);
        });
    }

    return (
        <div className="sign-up-container">

            {(errors && errors.length > 0) &&
                errors.map((err, i) => <span key={i}>{err}</span>)
            }

            <form onSubmit={handleSubmit}>
                <TextField
                    label="UserName"
                    value={userName}
                    type="text"
                    onChange={(e) => setUsername(e.currentTarget.value)}
                    validateOnFocusOut
                    validateOnLoad={false}
                    required />

                <TextField
                    label="Email"
                    value={email}
                    type="text"
                    onChange={(e) => setEmail(e.currentTarget.value)}
                    validateOnFocusOut
                    required />

                <TextField
                    label="Password"
                    value={passwordHash}
                    type="password"
                    onChange={(e) => setPasswordHash(e.currentTarget.value)}
                    validateOnFocusOut
                    canRevealPassword
                    required />

                <TextField
                    label="Confirm Password"
                    value={confirmPasswordHash}
                    type="password"
                    onChange={(e) => setConfirmPasswordHash(e.currentTarget.value)}
                    validateOnFocusOut
                    canRevealPassword
                    required />

                <button>Sign Up</button>
            </form>
        </div >
    )
}

export default SignUp;