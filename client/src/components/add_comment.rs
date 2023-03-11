use reqwasm::http::Request;
use serde::{Deserialize, Serialize};
use std::ops::Deref;
use wasm_bindgen::{JsCast, JsValue};
use web_sys::console;
use yew::events::InputEvent;
use yew::html::Properties;
use yew::prelude::*;

use super::comment::CommentProps;

#[derive(Deserialize, Clone, PartialEq, Properties, Serialize, Default)]
pub struct NewComment {
    pub postId: i32,
    pub content: String,
    pub userName: String,
}

#[derive(Clone, PartialEq, Properties)]
pub struct Props {
    pub postId: i32,
    pub on_add_comment: Callback<i8>,
}

#[function_component]
pub fn AddComment(props: &Props) -> Html {
    let comment_state = use_state(|| NewComment::default());
    let comment = comment_state.clone();
    let cloned_props = props.clone();

    let onsubmit = Callback::from(move |e: SubmitEvent| {
        e.prevent_default();

        let comment = comment.deref().clone();
        let cloned_props = cloned_props.clone();

        wasm_bindgen_futures::spawn_local({
            async move {
                let new_comment = CommentProps {
                    content: comment.content,
                    userName: "user123".to_string(),
                    postId: cloned_props.postId,
                    date: "".to_string(),
                };

                let req = Request::post("https://localhost:7123/api/comments")
                    .header("Content-Type", "application/json")
                    .body(serde_json::to_string(&new_comment).unwrap())
                    .send()
                    .await;

                match req {
                    Ok(_) => {
                        // tell parent component (posts.rs) that a new comment has been added
                        cloned_props.on_add_comment.emit(1);
                    }
                    Err(_) => console::log_1(&JsValue::from("Failed to create comment")),
                }
            }
        });
    });

    let oninput = Callback::from(move |e: InputEvent| {
        let input = &e
            .target()
            .unwrap()
            .dyn_into::<web_sys::HtmlTextAreaElement>();

        let mut data = comment_state.deref().clone();
        data.content = input.clone().unwrap().value();
        comment_state.set(data);
        // console::log_1(&JsValue::from(input.clone().unwrap().value()));
    });

    html! {
        <div>
            <form onsubmit={onsubmit}>
                <textarea
                    placeholder="your comment here..."
                    required={true}
                    oninput={oninput}
                >
                </textarea>
                <button>{ "Add Comment" }</button>
            </form>
        </div>
    }
}
