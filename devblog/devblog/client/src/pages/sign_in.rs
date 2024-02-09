use crate::{components::items::text_input::TextInput, helpers, ApiPost, User, UserField};
use stylist::Style;
use yew::prelude::*;
use yew_router::hooks::use_navigator;

const STYLE: &str = include_str!("styles/signIn.css");

#[function_component(SignIn)]
pub fn sign_in() -> Html {
    let style = Style::new(STYLE).unwrap();
    let user = use_state(User::default);
    let nav = use_navigator().unwrap();

    html! {
        <div class={style}>
            <div class="sign-in-container">

                // {error && <span>{error}</span>}

                <form onsubmit={helpers::onsubmit(&user, nav, ApiPost::SignIn)} class="sign-in">
                    <TextInput label="username" input_type="text" value={user.username.clone()} onchange={helpers::onchange(&user, UserField::Username)}/>
                    <TextInput label="password" input_type="password" value={user.password.clone()} onchange={helpers::onchange(&user, UserField::Password)}/>
                    <button>{"Login"}</button>
                </form>
            </div>
        </div>
    }
}
