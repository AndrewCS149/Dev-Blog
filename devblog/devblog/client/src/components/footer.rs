use crate::store::Store;
use stylist::Style;
use yew::{function_component, html, Html};
use yewdux::prelude::*;

const STYLE: &str = include_str!("styles/footer.css");

#[function_component(Footer)]
pub fn footer() -> Html {
    let style = Style::new(STYLE).unwrap();
    let store = use_store_value::<Store>();

    html! {
        <div class={style}>
            <div class="footer">
                if store.authenticated {
                    <span>{"Welcome "}{store.username.clone()}</span>
                }
            </div>
        </div>
    }
}
