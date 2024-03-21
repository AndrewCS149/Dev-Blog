use crate::{
    components::text_input::TextInput, helpers, router::Route, Api, IdentityError, Store, User,
    UserField,
};
use gloo_net::http::{Headers, Method};
use std::ops::Deref;
use stylist::Style;
use yew::prelude::*;
use yew_router::hooks::use_navigator;
use yewdux::prelude::*;

const STYLE: &str = include_str!("styles/signIn.css");

#[function_component(SignIn)]
pub fn sign_in() -> Html {
    let style = Style::new(STYLE).unwrap();
    let user = use_state(User::default);
    let error = use_state(|| IdentityError::default());
    let nav = use_navigator().unwrap();
    let (_, dispatch) = use_store::<Store>();

    let sign_in = {
        let user = user.deref().clone();
        let dispatch = dispatch.clone();
        let nav = nav.clone();
        let error = error.clone();
        Callback::from(move |e: SubmitEvent| {
            e.prevent_default();
            let nav = nav.clone();
            let dispatch = dispatch.clone();
            let error = error.clone();
            let user = user.clone();

            let hdrs = Headers::new();
            hdrs.append("content-type", "application/json");
            wasm_bindgen_futures::spawn_local(async move {
                let body = Some(helpers::to_jsvalue(user));

                if let Some(res) = Api::SignIn.fetch(Some(hdrs), body, Method::POST).await {
                    // navigate home if the submission is successful
                    if res.status() == 200 {
                        let obj: Store = serde_json::from_str(&res.text().await.unwrap()).unwrap();
                        helpers::set_user_data(dispatch, obj);
                        nav.push(&Route::Home);
                    } else {
                        let txt = res.text().await.unwrap();
                        error.set(serde_json::from_str(&txt).unwrap());
                    }
                }
            });
        })
    };

    html! {
        <div class={style}>
            <div class="sign-in-container">
                <form onsubmit={sign_in} class="sign-in">
                    <TextInput label="Username" input_type="text" value={user.username.clone()} onchange={helpers::on_change(&user, UserField::Username)}/>
                    <TextInput label="Password" input_type="password" value={user.password.clone()} onchange={helpers::on_change(&user, UserField::Password)}/>
                    <button>{"Login"}</button>
                </form>

                if !error.description.is_empty() {
                    <p>{error.description.deref()}</p>
                }
            </div>
        </div>
    }
}
