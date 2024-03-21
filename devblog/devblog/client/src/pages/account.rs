use crate::{
    helpers::{self, CustomCallback},
    store::Store,
    Api, User,
};
use gloo::utils::window;
use gloo_net::http::Method;
use std::rc::Rc;
use stylist::Style;
use yew::prelude::*;
use yew_router::{hooks::use_navigator, navigator::Navigator};
use yewdux::prelude::*;

const STYLE: &str = include_str!("styles/account.css");

#[function_component(Account)]
pub fn account() -> Html {
    let style = Style::new(STYLE).unwrap();
    let user = use_state(User::default);
    let user_cb = CustomCallback::new(&user);
    let nav = use_navigator().unwrap();
    let store = use_store_value::<Store>();
    let (_, dispatch) = use_store::<Store>();

    // get current user
    let token = store.token.clone();
    use_effect_with((), |_| {
        wasm_bindgen_futures::spawn_local(async move {
            let hdrs = helpers::create_auth_header(&token);
            let res = Api::GetCurrentUser
                .fetch(Some(hdrs), None, Method::GET)
                .await;

            helpers::emit(&user_cb, res.unwrap()).await;
        })
    });

    let on_click =
        |api: Api, method: Method, nav: Option<Navigator>, dispatch: Option<Dispatch<Store>>| {
            let token = store.token.clone();
            let api = Rc::new(api);
            Callback::from(move |_| {
                let mut confirm = true;
                if method == Method::DELETE {
                    confirm = window().confirm_with_message("Are you sure?").unwrap();
                }

                if confirm {
                    helpers::on_click(
                        token.clone(),
                        api.clone(),
                        method.clone(),
                        nav.clone(),
                        dispatch.clone(),
                    )
                }
            })
        };

    html! {
        <section class={style}>
            <div class="account">
                <div>
                    <h1>{"Account Details"}</h1>
                    <p>{"Username: "}{&user.username}</p>
                    <p>{"Email: "}{&user.email}</p>
                </div>
                <div>
                    <h1>{"Email Preferences"}</h1>
                    <input type="checkbox"
                        checked={user.subscribed}
                        onclick={on_click(Api::ToggleSubscribe, Method::PUT, None, None)}/>
                    <span>{"Subscribe to Newsletter"}</span>
                </div>
                <button onclick={on_click(Api::DeleteCurrentAccount, Method::DELETE, Some(nav), Some(dispatch))}>{"Delete Account"}</button>
            </div>
        </section>
    }
}
