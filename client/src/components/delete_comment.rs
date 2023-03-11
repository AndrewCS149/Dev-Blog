use std::{ops::Deref, str::FromStr};

use js_sys::JsString;
use reqwasm::http::Request;
use wasm_bindgen::JsValue;
use web_sys::console;
use yew::prelude::*;

#[derive(PartialEq, Properties, Clone)]
pub struct Props {
    pub id: i32,
    pub on_delete: Callback<i32>,
}

#[function_component]
pub fn DeleteComment(props: &Props) -> Html {
    let comment_id = use_state(|| props.id.clone());
    let cloned_props = props.clone();

    let onclick = Callback::from(move |_| {
        let comment_id = comment_id.deref().clone();
        let cloned_props = cloned_props.clone();

        wasm_bindgen_futures::spawn_local({
            async move {
                let id_str = comment_id.to_string();
                let url = format!("https://localhost:7123/api/comments/{}", &id_str);
                let res = Request::delete(&url).send().await;
                // .unwrap()
                // .text()
                // .await
                // .unwrap();

                match res {
                    Ok(_) => {
                        // tell parent component (posts.rs) that a new comment has been added
                        cloned_props.on_delete.emit(1);
                    }
                    Err(_) => console::log_1(&JsValue::from("Failed to delete comment")),
                }
            }
        });
    });

    html! {
        <button onclick={onclick}>{"Delete"}</button>
    }
}
