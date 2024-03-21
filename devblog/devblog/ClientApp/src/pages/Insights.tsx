import { useEffect, useState } from "react";
import "./styles/Insights.css";
import { MdDelete as Trash } from "react-icons/md";

interface IUserInfo {
    userName: string,
    email: string,
    subscribed: boolean
}

const Insights = () => {
    const [users, setUsers] = useState<IUserInfo[]>();
    const [subscribedUsers, setSubscribedUsers] = useState<number>(0);

    const getUsers = async () => {
        await fetch("api/accounts", {
            method: "GET",
            headers: { "Authorization": `Bearer ${localStorage.getItem("token")}` }
        }).then((res) => { return res.json(); })
            .then((data) => {
                setUsers(data);

                // count subscribed users
                const subscribedCount = data.reduce((count: number, user: IUserInfo) => {
                    return user.subscribed ? count + 1 : count;
                }, 0);
                setSubscribedUsers(subscribedCount);
            });
    }

    const handleDeleteAccount = async (userName: string) => {
        const shouldDelete = window.confirm(`Are you sure you would like to delete user ${userName}?`);

        if (shouldDelete) {
            await fetch(`api/accounts/adminDelete/${userName}`, {
                method: "DELETE",
                headers: {
                    "Content-Type": "application/json",
                    "Authorization": `Bearer ${localStorage.getItem("token")}`
                }
            }).then(() => {
                setUsers(users?.filter((user) => user.userName !== userName));
            })
                .catch((e) => console.log(`Error deleting account: ${e}`));
        }
    }

    useEffect(() => { getUsers() }, []);

    return (
        <section className="insights">
            <p>Total Users: {users?.length}</p>
            <p>Subscribed Users: {subscribedUsers}</p>

            <table className="user-table">
                <thead>
                    <tr>
                        <th>Username</th>
                        <th>Email</th>
                        <th>Subscribed</th>
                        <th>Delete</th>
                    </tr>
                </thead>
                <tbody>
                    {users?.map((user, index) => (
                        <tr key={index}>
                            <td>{user.userName}</td>
                            <td>{user.email}</td>
                            <td>{user.subscribed ? 'Yes' : 'No'}</td>
                            <td><Trash className="delete-post-btn" onClick={() => handleDeleteAccount(user.userName)} /></td>
                        </tr>
                    ))}
                </tbody>
            </table>
        </section>
    )
}

export default Insights;