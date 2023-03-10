use js_sys::JsString;
use reqwasm::http::Request;
use serde::Deserialize;
use web_sys::console;
use yew::html::Properties;
use yew::prelude::*;

#[derive(Deserialize, Clone, PartialEq, Properties)]
pub struct PostProps {
    pub id: i32,
    pub updateNum: String,
    pub description: String,
    pub date: String,
    pub imgURL: String,
    pub comments: Vec<CommentProps>,
}

#[derive(Deserialize, Clone, PartialEq, Properties)]
pub struct CommentProps {
    pub id: i32,
    pub content: String,
    pub date: String,
    pub userName: String,
}

#[function_component]
fn Comment(props: &CommentProps) -> Html {
    html! {
        <div>
            <div>
                <span>{&props.userName}</span>
                <span>{&props.date}</span>
            </div>

            <p>{&props.content}</p>
        </div>
    }
}

#[function_component]
fn Post(props: &PostProps) -> Html {
    html! {
        <div>
            <div>
                <span>{&props.updateNum}</span>
                <span>{&props.date}</span>
            </div>

            <img src={props.imgURL.clone()}/>
            <p>{&props.description}</p>

            <div>
                {for props.comments.iter().map(|comment| html! {
                    <Comment
                        id={comment.id.clone()}
                        content={comment.content.clone()}
                        date={comment.date.clone()}
                        userName={comment.userName.clone()}
                    />
                })}
            </div>
        </div>
    }
}

#[function_component(Posts)]
pub fn posts() -> Html {
    let posts = use_state(|| Vec::<PostProps>::new());

    if posts.is_empty() {
        wasm_bindgen_futures::spawn_local({
            let posts = posts.clone();

            async move {
                let data = Request::get(&"https://localhost:7123/api/posts")
                    .send()
                    .await
                    .unwrap()
                    .text()
                    .await
                    .unwrap();

                posts.set(serde_json::from_str(&data).unwrap());
                console::log_1(&JsString::from(data));
            }
        });
    }

    html! {
        <section>
            <h1>{"POSTS"}</h1>
            { for posts.iter().map(|post| html! {
                <Post
                    id={post.id.clone()}
                    updateNum={post.updateNum.clone()}
                    description={post.description.clone()}
                    imgURL={post.imgURL.clone()}
                    date={post.date.clone()}
                    comments={post.comments.clone()}
                />
            }) }
        </section>
    }
}
