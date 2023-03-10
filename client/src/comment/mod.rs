// use reqwasm::http::Request;
// use serde::Deserialize;
// use yew::prelude::*;

// #[derive(Deserialize)]
// struct Comment {
//     id: i32,
//     content: String,
//     date: String,
//     userName: String,
// }

// #[function_component(Posts)]
// pub fn comment() -> Html {
//     let posts = use_state(|| Vec::<Post>::new());

//     if posts.is_empty() {
//         wasm_bindgen_futures::spawn_local({
//             let posts = posts.clone();

//             async move {
//                 let data = Request::get(&"https://localhost:7123/api/posts")
//                     .send()
//                     .await
//                     .unwrap()
//                     .text()
//                     .await
//                     .unwrap();

//                 posts.set(serde_json::from_str(&data).unwrap());
//             }
//         });
//     }

//     html! {
//         <section>

//         </section>
//     }
// }
