use js_sys::JsString;
use reqwasm::http::Request;
use web_sys::console;
use yew::prelude::*;

use crate::components::post::*;

#[function_component(Home)]
pub fn home() -> Html {
    let post = use_state(|| PostProps::default());

    if post.id == 0 {
        wasm_bindgen_futures::spawn_local({
            let post = post.clone();

            async move {
                let data = Request::get(&"https://localhost:7123/api/posts/-1")
                    .send()
                    .await
                    .unwrap()
                    .text()
                    .await
                    .unwrap();

                post.set(serde_json::from_str(&data).unwrap());
                // console::log_1(&JsString::from(data));
            }
        });
    }

    html! {
        <div>
        <h1>{"HOME"}</h1>
            {if post.id != 0 {
                html! {
                    <Post
                        id={post.id.clone()}
                        updateNum={post.updateNum.clone()}
                        description={post.description.clone()}
                        imgURL={post.imgURL.clone()}
                        date={post.date.clone()}
                        comments={post.comments.clone()}
                    />
                }
            } else {
                html! {
                    <p>{"Loading..."}</p>
                }
            }}
        </div>
    }
}
