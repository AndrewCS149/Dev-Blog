use crate::{helpers, icons::icons::EditIcon, store::Store, Api};
use gloo_net::http::Method;
use std::ops::Deref;
use stylist::Style;
use web_sys::{HtmlTextAreaElement, InputEvent, SubmitEvent};
use yew::{
    function_component, html, use_effect_with, use_state, Callback, Html, Properties, TargetCast,
};
use yewdux::use_store_value;

const STYLE: &str = include_str!("styles/postEdit.css");

#[derive(Properties, PartialEq)]
pub struct Props {
    pub id: u32,
    pub description: String,
    pub on_post_edit: Callback<String>,
}

#[function_component(EditPost)]
pub fn edit_post(props: &Props) -> Html {
    let style = Style::new(STYLE).unwrap();
    let description = use_state(|| props.description.clone());
    let char_count = use_state(|| 0);
    let is_editing = use_state(|| false);
    let store = use_store_value::<Store>();

    let update_post = {
        let description = description.deref().clone();
        let token = store.token.clone();
        let is_editing = is_editing.clone();
        let id = props.id.clone();
        Callback::from(move |e: SubmitEvent| {
            e.prevent_default();
            let description = description.clone();
            let body = Some(helpers::to_jsvalue(description.deref()));
            let hdrs = helpers::create_auth_header(&token);
            hdrs.append("content-type", "application/json");
            let is_editing = is_editing.clone();

            wasm_bindgen_futures::spawn_local(async move {
                let response = Api::EditPost(id).fetch(Some(hdrs), body, Method::PUT).await;

                if let Some(res) = response {
                    if res.status() == 200 {
                        is_editing.set(false);
                    }
                }
            });
        })
    };

    let on_description_change = {
        let on_post_edit = props.on_post_edit.clone();
        let description = description.clone();
        Callback::from(move |e: InputEvent| {
            let input = e.target_dyn_into::<HtmlTextAreaElement>();
            if let Some(value) = input {
                description.set(value.value());
                on_post_edit.emit(value.value());
            }
        })
    };

    let on_edit = {
        let is_editing = is_editing.clone();
        let description = description.clone();
        let d: String = props.description.clone();
        Callback::from(move |_| {
            is_editing.set(true);
            description.set(d.clone());
        })
    };

    let char_count_clone = char_count.clone();
    let description_clone = description.clone();
    use_effect_with(description_clone.clone(), move |_| {
        char_count_clone.set(description_clone.deref().len());
    });

    html! {
        <div class={style}>
            if store.admin {
                if !*is_editing {
                    <span class="edit-post-btn" onclick={on_edit}>
                        <EditIcon />
                    </span>
                } else {
                    <div class="edit-post">
                        <form onsubmit={update_post}>
                            <p>{"Mastodon char count: "}{*char_count}{"/500"}</p>
                            <textarea class="edit-post-description" value={description.deref().clone()} oninput={on_description_change} />
                            <button>{"Update"}</button>
                            <button onclick={move |_| {is_editing.clone().set(false)}}>{"Cancel"}</button>
                        </form>
                    </div>
                }
            }
        </div>
    }
}
