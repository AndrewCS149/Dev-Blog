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

const STYLE: &str = include_str!("styles/signUp.css");

#[function_component(SignUp)]
pub fn sign_up() -> Html {
    let style = Style::new(STYLE).unwrap();
    let user = use_state(User::default);
    let errors = use_state(|| Vec::<IdentityError>::new());
    let nav = use_navigator().unwrap();
    let (_, dispatch) = use_store::<Store>();

    // confirm password match
    let user_clone = user.clone();
    let errors_clone = errors.clone();
    use_effect_with(
        (
            user_clone.password_hash.clone(),
            user_clone.password_hash_confirm.clone(),
        ),
        move |(password, password_confirm)| {
            let mut cur_errors = errors_clone.deref().clone();
            let description = "Passwords do not match".to_string();
            if password != password_confirm {
                cur_errors.push(IdentityError {
                    code: "".to_string(),
                    description: description,
                });
            } else {
                if let Some(idx) = cur_errors.iter().position(|e| e.description == description) {
                    cur_errors.remove(idx);
                }
            }

            errors_clone.set(cur_errors);
        },
    );

    let sign_up = {
        let user = user.deref().clone();
        let dispatch = dispatch.clone();
        let errors = errors.clone();
        Callback::from(move |e: SubmitEvent| {
            e.prevent_default();
            let nav = nav.clone();
            let dispatch = dispatch.clone();
            let errors = errors.clone();
            let user = user.clone();
            let hdrs = Headers::new();
            hdrs.append("content-type", "application/json");
            wasm_bindgen_futures::spawn_local(async move {
                let body = Some(helpers::to_jsvalue(user));

                if let Some(res) = Api::SignUp.fetch(Some(hdrs), body, Method::POST).await {
                    // navigate home if the submission is successful
                    if res.status() == 200 && errors.deref().len() == 0 {
                        let obj: Store = serde_json::from_str(&res.text().await.unwrap()).unwrap();
                        helpers::set_user_data(dispatch, obj);
                        nav.push(&Route::Home);
                    } else {
                        let txt = res.text().await.unwrap();
                        errors.set(serde_json::from_str(&txt).unwrap());
                    }
                }
            });
        })
    };

    html! {
        <div class={style}>
            <div class="sign-up-container">
                <form onsubmit={sign_up}>
                    <TextInput label="Username" input_type="text" value={user.username.clone()} onchange={helpers::on_change(&user, UserField::Username)}/>
                    <TextInput label="Email" input_type="text" value={user.email.clone()} onchange={helpers::on_change(&user, UserField::Email)}/>
                    <TextInput label="Password" input_type="password" value={user.password_hash.clone()} onchange={helpers::on_change(&user, UserField::PasswordHash)}/>
                    <TextInput label="Confirm Password" input_type="password" value={user.password_hash_confirm.clone()} onchange={helpers::on_change(&user, UserField::PasswordHashConfirm)}/>
                    <button>{"Sign Up"}</button>
                </form>

                if errors.len() > 0 {
                    <div class="errors">
                        {for errors.iter().enumerate().map(|(_idx, e)| html! {
                            <p>{&e.description}</p>
                        })}
                    </div>
                }
            </div>
        </div>
    }
}
