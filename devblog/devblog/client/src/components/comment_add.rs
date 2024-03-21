use crate::{helpers, store::Store, Api, CommentModel};
use gloo::console::log;
use gloo_net::http::Method;
use std::ops::Deref;
use stylist::Style;
use web_sys::{Event, HtmlTextAreaElement, SubmitEvent};
use yew::prelude::*;
use yewdux::prelude::*;

const STYLE: &str = include_str!("styles/commentAdd.css");

#[derive(Properties, PartialEq)]
pub struct Props {
    pub post_id: u32,
    pub on_comment_add: Callback<CommentModel>,
}

#[function_component(AddComment)]
pub fn add_comment(props: &Props) -> Html {
    let style = Style::new(STYLE).unwrap();
    let comment = use_state(CommentModel::default);
    let store = use_store_value::<Store>();

    let onchange = {
        let comment = comment.clone();
        Callback::from(move |e: Event| {
            let mut comment_clone = comment.deref().clone();
            let input = e.target_dyn_into::<HtmlTextAreaElement>();
            if let Some(value) = input {
                comment_clone.content = value.value();
                comment.set(comment_clone);
            }
        })
    };

    let onsubmit = {
        let comment = comment.clone();
        let on_comment_add = props.on_comment_add.clone();
        let post_id = props.post_id.clone();
        let store = store.clone();
        Callback::from(move |e: SubmitEvent| {
            e.prevent_default();
            let hdrs = helpers::create_auth_header(&store.token);
            hdrs.append("content-type", "application/json");
            let comment = comment.clone();

            let new_comment = CommentModel::new(
                post_id,
                comment.deref().content.clone(),
                store.username.clone(),
            );

            let body = Some(helpers::to_jsvalue(new_comment.clone()));
            log!("Response: ", body.clone().unwrap());

            let on_comment_add = on_comment_add.clone();
            wasm_bindgen_futures::spawn_local(async move {
                let response = Api::AddComment.fetch(Some(hdrs), body, Method::POST).await;

                if let Some(res) = response {
                    if res.status() == 200 {
                        let txt = &res.text().await.unwrap();
                        let data = serde_json::from_str::<CommentModel>(&txt).unwrap();
                        on_comment_add.emit(data);
                        comment.set(CommentModel::default());
                    }
                }
            });
        })
    };

    html! {
        <div class={style}>
            <div class="create-comment">
                <form {onsubmit}>
                    if store.authenticated.clone() {
                        <textarea placeholder="your comments here..."
                            type="text"
                            required={true}
                            value={comment.content.clone()}
                            onchange={onchange}>
                        </textarea>
                        <button>{"Add Comment"}</button>
                    } else {
                        <textarea placeholder="sign in to comment..." disabled={true}></textarea>
                    }
                </form>
            </div>
        </div>
    }
}
