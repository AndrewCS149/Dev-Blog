use chrono::NaiveDateTime;
use js_sys::JsString;
use reqwasm::http::Request;
use serde::{Deserialize, Serialize};
use web_sys::console;
use yew::prelude::*;

#[derive(Deserialize, Debug)]
struct Post {
    id: i32,
    updateNum: String,
    description: String,
    // date: NaiveDateTime,
    imgURL: String,
}

#[function_component(Posts)]
pub fn posts() -> Html {
    let posts = use_state(|| Vec::<Post>::new());

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
                <div>
                    <div>
                        <span>{&post.updateNum}</span>
                    </div>

                    <img src={post.imgURL.clone()}/>
                    <p>{&post.description}</p>
                </div>
            }) }
        </section>
    }
}
