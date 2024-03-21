use crate::{helpers, icons::icons::EditIcon, store::Store, Api};
use gloo_net::http::Method;
use std::ops::Deref;
use stylist::Style;
use web_sys::{Event, HtmlTextAreaElement};
use yew::{function_component, html, use_state, Callback, Html, Properties, TargetCast};
use yewdux::use_store_value;

const STYLE: &str = include_str!("styles/commentEdit.css");

#[derive(Properties, PartialEq)]
pub struct Props {
    pub id: u32,
    pub content: String,
    pub is_editing: bool,
    pub on_is_editing: Callback<bool>,
    pub on_edit_save: Callback<String>,
}

#[function_component(EditComment)]
pub fn edit_comment(props: &Props) -> Html {
    let style = Style::new(STYLE).unwrap();
    let content = use_state(|| props.content.clone());
    let store = use_store_value::<Store>();

    let onchange = {
        let on_is_editing = props.on_is_editing.clone();
        let content = content.clone();
        Callback::from(move |e: Event| {
            on_is_editing.emit(true);
            let input = e.target_dyn_into::<HtmlTextAreaElement>();
            if let Some(value) = input {
                content.set(value.value());
            }
        })
    };

    let cancel = {
        let on_is_editing = props.on_is_editing.clone();
        Callback::from(move |_| on_is_editing.emit(false))
    };

    let save = {
        let on_is_editing = props.on_is_editing.clone();
        let id = props.id.clone();
        let on_save = props.on_edit_save.clone();
        let content = content.deref().clone();
        Callback::from(move |_| {
            on_is_editing.emit(false);
            let content = content.clone();
            let on_save = on_save.clone();
            let body = Some(helpers::to_jsvalue(content.deref()));
            let hdrs = helpers::create_auth_header(&store.token);
            hdrs.append("content-type", "application/json");

            wasm_bindgen_futures::spawn_local(async move {
                let response = Api::EditComment(id)
                    .fetch(Some(hdrs), body, Method::PUT)
                    .await;

                if let Some(res) = response {
                    if res.status() == 200 {
                        on_save.emit(content);
                    }
                }
            });
        })
    };

    let edit = {
        let on_is_editing = props.on_is_editing.clone();
        Callback::from(move |_| on_is_editing.emit(true))
    };

    html! {
        <>
            <div class={style}>
                if !props.is_editing {
                    <span class="edit-comment-btn" onclick={edit}>
                        <EditIcon/>
                    </span>
                }

                if props.is_editing {
                    <div class="edit-comment">
                        <textarea value={content.deref().clone()} {onchange}></textarea>

                        <div>
                            <button onclick={save}>{"Save"}</button>
                            <button onclick={cancel}>{"Cancel"}</button>
                        </div>
                    </div>
                }
            </div>
        </>
    }
}
