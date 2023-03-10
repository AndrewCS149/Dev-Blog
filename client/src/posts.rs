use js_sys::JsString;
use reqwasm::http::Request;
use web_sys::console;
use yew::prelude::*;

use crate::components::post::*;

// gets all posts from API
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

            // render all posts
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
