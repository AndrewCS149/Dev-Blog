import { useCallback, useEffect, useState } from "react";
import { MdChevronLeft } from "react-icons/md"
import { MdChevronRight } from "react-icons/md"
import IPost from "../interfaces/IPostProps";
import Post from "../components/Post";
import { GetIsAdmin } from "../components/AuthenticationService";
import { Link, useLocation } from "react-router-dom";
import "./styles/Posts.css";

const Posts = () => {
    const [posts, setPosts] = useState<IPost[]>([]);
    const [isAdmin, setIsAdmin] = useState(false);
    const [pageNum, setPageNum] = useState(1);
    const [totalPages, setTotalPages] = useState(0);
    const [totalPosts, setTotalPosts] = useState(0);
    const [pageParamQuery, setPageParamQuery] = useState(0);
    const location = useLocation();
    const searchParams = new URLSearchParams(location.search);
    const pageParam = searchParams.get("pageNum");

    useEffect(() => {
        fetch(`api/posts/page/${pageNum}`)
            .then((res) => { return res.json(); })
            .then((data) => setPosts(data))
            .catch((e) => console.log("Error retrieving posts: " + e))

    }, [pageNum]);

    useEffect(() => {
        console.log("HELLO");

        if (pageParam) {
            setPageNum(parseInt(pageParam));
            setPageParamQuery(parseInt(pageParam));
        }
    }, [location.search, pageParam]);

    useEffect(() => {
        setIsAdmin(GetIsAdmin);
        fetch("api/posts/page/count")
            .then((res) => { return res.json(); })
            .then((data) => {
                setTotalPages(data)
            }).catch((e) => console.log("Error retrieving page count: " + e));

        fetch("api/posts/count")
            .then((res) => { return res.json(); })
            .then((data) => {
                setTotalPosts(data)
            }).catch((e) => console.log("Error retrieving post count: " + e));
    }, []);

    const scrollToHash = () => {
        const postIdParam = searchParams.get("postId");
        const target = document.querySelector("#post" + postIdParam);
        if (target) {
            target.scrollIntoView({ behavior: "smooth" });
        }
    };

    useEffect(() => {
        if (pageNum === pageParamQuery) {
            setTimeout(() => {
                scrollToHash();
            }, 1700);
        }

    }, [scrollToHash, pageNum, pageParamQuery]);

    const handlePagerClick = (page: number) => {
        window.history.replaceState(null, '', '/posts');
        setPageParamQuery(-1);
        setPageNum(page);
    }

    const Pager = () => {
        return <div className="pager">
            {pageNum > 1 ? (
                <MdChevronLeft className="arrow-visible" onClick={() => pageNum > 0 && handlePagerClick(pageNum - 1)} />
            ) : (
                <MdChevronLeft className="arrow-hidden" />
            )}

            <span>{pageNum}</span>

            {pageNum < totalPages ? (
                <MdChevronRight className="arrow-visible" onClick={() => handlePagerClick(pageNum + 1)} />
            ) : (
                <MdChevronRight className="arrow-hidden" />
            )}
        </div>
    }

    return (
        <section className="posts">
            <Pager />
            {isAdmin && <Link className="create-post-btn" to="/posts/create">Create Post</Link>}
            {posts.length === 0 ? <h1>Loading...</h1> : posts.map((p, i) => <Post key={p.id} {...p} postNumber={totalPosts - 5 * (pageNum - 1) - i} />)}
            <Pager />
        </section>
    );
}

export default Posts;
