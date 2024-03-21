import { useCallback, useEffect, useState } from "react";
import { GetUserName, IsLoggedIn } from "./AuthenticationService";
import { SentimentVerySatisfied as UpVote } from '@mui/icons-material';
import { SentimentDissatisfied as DownVote } from '@mui/icons-material';
import { Tooltip } from '@mui/material';
import "./styles/Vote.css";

interface IVoteProps {
    postId?: number;
}

const Vote = (props: IVoteProps) => {
    const [username, setUsername] = useState("");
    const [downVotes, setDownVotes] = useState<number>();
    const [upVotes, setUpVotes] = useState<number>();
    const [loggedIn, setIsLoggedIn] = useState<boolean>();

    const getVotes = useCallback(async (vote: string) => {
        await fetch(`api/posts/${props.postId}/${vote}`, {
            method: "GET",
            headers: {
                "Content-Type": "application/json",
                "Authorization": `Bearer ${localStorage.getItem("token")}`
            },
        }).then((res) => { return res.json() })
            .then((data) => {
                if (vote === "upvotes") {
                    setUpVotes(data);
                } else {
                    setDownVotes(data);
                }
            }).catch((e) => console.log(e));
    }, [props.postId]);

    useEffect(() => {
        setUsername(GetUserName);
        setIsLoggedIn(IsLoggedIn);
        getVotes("upvotes");
        getVotes("downvotes");
    }, [upVotes, downVotes, getVotes]);

    const handleVote = async (vote: string) => {
        await fetch(`api/posts/${props.postId}/${vote}`, {
            method: "POST",
            headers: {
                "Content-Type": "application/json",
                "Authorization": `Bearer ${localStorage.getItem("token")}`
            },
        }).then((res) => {
            getVotes("upvotes");
            getVotes("downvotes");
            return res.json()
        }).catch((e) => console.log(e));
    };

    return (
        <div className="votes">
            <Tooltip title={loggedIn ? '' : 'Login to like/dislike'} arrow>
                <UpVote style={{ fontSize: 30 }} onClick={() => handleVote("upvote")} className="up-vote" />
            </Tooltip>
            <span>{upVotes}</span>

            <Tooltip title={loggedIn ? '' : 'Login to like/dislike'} arrow>
                <DownVote style={{ fontSize: 30 }} onClick={() => handleVote("downvote")} className="down-vote" />
            </Tooltip>
            <span>{downVotes}</span>
        </div>
    )
}

export default Vote;