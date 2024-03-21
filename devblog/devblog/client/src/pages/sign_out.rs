use crate::{helpers, router::Route, Api, Store, User};
use gloo_net::http::{Headers, Method};
use std::ops::Deref;
use stylist::Style;
use yew::prelude::*;
use yew_router::hooks::use_navigator;
use yewdux::prelude::*;

const STYLE: &str = include_str!("styles/signOut.css");

#[function_component(SignOut)]
pub fn sign_out() -> Html {
    let style = Style::new(STYLE).unwrap();
    let user = use_state(User::default);
    let nav = use_navigator().unwrap();
    let (_, dispatch) = use_store::<Store>();

    let sign_out = {
        let user = user.clone();
        Callback::from(move |e: SubmitEvent| {
            e.prevent_default();

            let dispatch = dispatch.clone();
            let nav = nav.clone();
            let user = user.deref().clone();

            // build headers
            let hdrs = Headers::new();
            hdrs.append("content-type", "application/json");

            wasm_bindgen_futures::spawn_local(async move {
                let body = Some(helpers::to_jsvalue(user));

                // navigate home if the submission is successful
                if let Some(res) = Api::SignOut.fetch(Some(hdrs), body, Method::POST).await {
                    if res.status() == 200 {
                        let obj: Store = serde_json::from_str(&res.text().await.unwrap()).unwrap();
                        dispatch.reduce(|_| Store::default().into());
                        helpers::set_user_data(dispatch, obj);
                        nav.push(&Route::Home);
                    }
                }
            });
        })
    };

    html! {
        // <form class={style} onsubmit={helpers::on_submit(&user, nav, Rc::new(Api::SignOut), dispatch)}>
        <form class={style} onsubmit={sign_out}>
            <button class="logout">{"Logout"}</button>
        </form>
    }
}
