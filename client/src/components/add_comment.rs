use js_sys::JsString;
use reqwasm::http::Request;
use serde::{Deserialize, Serialize};
use web_sys::{console, FormData};
use yew::html::Properties;
use yew::prelude::*;

use super::comment::CommentProps;

// pub enum Msg {
//     SetContent(String),
// }

// pub struct State {
//     content: String,
//     userName: String,
// }

// impl Component for State {
//     type Message = Msg;
//     type Properties = ();

//     fn create(ctx: &Context<Self>) -> Self {
//         todo!()
//     }

//     fn view(&self, ctx: &Context<Self>) -> Html {
//         todo!()
//     }
// }

#[derive(Deserialize, Clone, PartialEq, Properties, Serialize)]
pub struct CommentP {
    pub id: i32,
    pub content: String,
    pub userName: String,
}

#[function_component]
pub fn AddComment() -> Html {
    let content = use_state(|| String::from(""));
    // console::log_1(&JsString::from(content.to_owned()));

    let handle_submit = |e: SubmitEvent| {
        e.prevent_default();

        wasm_bindgen_futures::spawn_local({
            async move {
                let new_comment = CommentP {
                    content: "this is the comment".to_string(),
                    userName: "user123".to_string(),
                    id: 1,
                    // date: "".to_string(),
                };

                let req = Request::post("https://localhost:7123/api/comments")
                    .header("Content-Type", "application/json")
                    .body(serde_json::to_string(&new_comment).unwrap());

                console::log_1(&"SUBMIT".to_string().into())
                // .body(Json(&Comment {
                //     content: "this is the comment",
                //     userName: "user123",
                //     postId: 1,
                // }));
                // .body(serde_json::from_value(&CommentProps {
                //     content: "this is the comment",
                //     userName: "user123",
                //     postId: 1,
                // }));
            }
        });
    };

    html! {
        <div>
            <form onsubmit={handle_submit}>
                <textarea
                    placeholder="your comment here..."
                    required={true}
                    // value={content}
                    // handle_onchange
                    // onChange
                >
                </textarea>
                <button>{ "Add Comment" }</button>
            </form>
        </div>
    }
}
