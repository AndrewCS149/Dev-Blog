use crate::{helpers, icons::icons::TrashIcon, store::Store, Api};
use gloo::utils::window;
use gloo_net::http::Method;
use stylist::Style;
use yew::{function_component, html, Callback, Html, Properties};
use yewdux::use_store_value;

const STYLE: &str = include_str!("styles/PostDelete.css");

#[derive(Properties, PartialEq)]
pub struct Props {
    pub id: u32,
    pub on_post_delete: Callback<u32>,
}

#[function_component(DeletePost)]
pub fn delete_post(props: &Props) -> Html {
    let style = Style::new(STYLE).unwrap();
    let store = use_store_value::<Store>();

    let delete = {
        let id = props.id.clone();
        let token = store.token.clone();
        let callback = props.on_post_delete.clone();
        Callback::from(move |_| {
            let confirm = window().confirm_with_message("Are you sure?").unwrap();

            if confirm {
                let hdrs = helpers::create_auth_header(&token);
                let callback = callback.clone();
                wasm_bindgen_futures::spawn_local(async move {
                    // delete post
                    let response = Api::DeletePost(id)
                        .fetch(Some(hdrs), None, Method::DELETE)
                        .await;

                    // invoke on_post_delete callback
                    if let Some(res) = response {
                        if res.status() == 200 {
                            callback.emit(id);
                        }
                    }
                });
            }
        })
    };

    html! {
        if store.admin {
            <span class={style}>
                <span onclick={delete} class="delete-post-btn">
                    <TrashIcon />
                </span>
            </span>
        }
    }
}
