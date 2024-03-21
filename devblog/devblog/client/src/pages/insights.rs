use crate::{
    helpers::{self, CustomCallback},
    icons::icons::TrashIcon,
    store::Store,
    Api, User,
};
use gloo::utils::window;
use gloo_net::http::Method;
use std::{ops::Deref, rc::Rc};
use stylist::Style;
use yew::prelude::*;
use yewdux::prelude::*;

const STYLE: &str = include_str!("styles/insights.css");

#[function_component(Insights)]
pub fn insights() -> Html {
    let style = Style::new(STYLE).unwrap();
    let users = use_state(|| vec![User::default()]);
    let users_cb = CustomCallback::new(&users);
    let store = use_store_value::<Store>();

    // get all users
    let token = store.token.clone();
    use_effect_with((), move |_| {
        wasm_bindgen_futures::spawn_local(async move {
            let hdrs = helpers::create_auth_header(&token);
            let res = Api::GetUsers.fetch(Some(hdrs), None, Method::GET).await;
            helpers::emit(&users_cb, res.unwrap()).await;
        });
    });

    html! {
        <section class={style}>
            <div class="insights">
                <p>{"Total Users: "}{users.len()}</p>
                <p>{"Subscribed Users: "}{users.iter().filter(|u| u.subscribed).count() as u32}</p>

                <table class="user-table">
                    <thead>
                        <tr>
                            <th>{"Username"}</th>
                            <th>{"Email"}</th>
                            <th>{"Subscribed"}</th>
                        </tr>
                    </thead>
                    <tbody>
                        {for users.iter().map(|user|{
                            let current_users = users.clone();
                            let onclick = |name: String| {


                                let token = store.token.clone();
                                let api = Rc::new(Api::DeleteAccount(user.username.clone()));
                                let users = users.clone();
                                Callback::from(move |_| {
                                    let confirm = window().confirm_with_message("Are you sure?").unwrap();
                                    if confirm {
                                        // remove deleted user from array
                                        let mut current_users = current_users.deref().clone();
                                        if let Some(idx) = current_users.iter().position(|u| u.username == name) {
                                            current_users.remove(idx);
                                            users.set(current_users);
                                        }
                                    }
                                    helpers::on_click(token.clone(), Rc::clone(&api), Method::DELETE, None, None);
                                })
                            };

                            html! {
                                <tr>
                                    <td>{&user.username}</td>
                                    <td>{&user.email}</td>
                                    <td>{if user.subscribed {"yes"} else {"no"}}</td>
                                    <td onclick={onclick(user.username.clone())}><TrashIcon /></td>
                                </tr>
                            }
                        })}
                    </tbody>
                </table>
            </div>
        </section>
    }
}
