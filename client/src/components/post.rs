use js_sys::JsString;
use reqwasm::http::Request;
use serde::Deserialize;
use std::ops::Deref;
use wasm_bindgen::JsValue;
use web_sys::console;
use yew::html::Properties;
use yew::prelude::*;

use crate::components::add_comment::AddComment;
use crate::components::comment::*;
use crate::components::delete_comment::DeleteComment;

#[derive(Deserialize, Clone, PartialEq, Properties)]
pub struct PostProps {
    pub id: i32,
    pub updateNum: String,
    pub description: String,
    pub date: String,
    pub imgURL: String,
    pub comments: Vec<CommentProps>,
}

impl Default for PostProps {
    fn default() -> Self {
        Self {
            id: 0,
            updateNum: "".to_string(),
            description: "".to_string(),
            date: "".to_string(),
            imgURL: "".to_string(),
            comments: Vec::new(),
        }
    }
}

#[function_component]
pub fn Post(props: &PostProps) -> Html {
    let comments_state = use_state(|| props.comments.to_owned());
    let comments = comments_state.clone();
    let post_id = use_state(|| props.id.clone());
    let change_add = use_state(|| 0);
    let change_delete = use_state(|| 0);

    // refresh comments if a new comment has been added
    use_effect_with_deps(
        move |_| {
            wasm_bindgen_futures::spawn_local({
                let comments_state = comments_state.clone();

                async move {
                    let id_str = post_id.to_string();
                    let url = format!("https://localhost:7123/api/comments/post/{}", &id_str);
                    let data = Request::get(&url)
                        .send()
                        .await
                        .unwrap()
                        .text()
                        .await
                        .unwrap();

                    comments_state.set(serde_json::from_str(&data).unwrap());
                    console::log_1(&JsString::from(data));
                }
            });
        },
        (change_add.clone(), change_delete.clone()),
    );

    let on_add = Callback::from(move |_| {
        let val = change_add.deref().clone();
        change_add.set(val + 1);
    });

    let on_delete = Callback::from(move |_| {
        let val = change_delete.deref().clone();
        change_delete.set(val + 1);
    });

    html! {
        <div>
            <div>
                <span>{&props.updateNum}</span>
                <span>{&props.date}</span>
            </div>

            <img src={props.imgURL.clone()}/>
            <p>{&props.description}</p>

            <div>
            {for comments.iter().map(|comment| html! {
                <div>
                    <DeleteComment id={comment.id} on_delete={on_delete.clone()}/>
                    <Comment
                        id={comment.id}
                        postId={props.id}
                        content={comment.content.clone()}
                        date={comment.date.clone()}
                        userName={comment.userName.clone()}
                    />
                    </div>
                })}
            </div>
            <AddComment postId={props.id} on_add={on_add}/>
        </div>
    }
}
