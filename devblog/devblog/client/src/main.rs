mod api;
mod components;
mod helpers;
mod icons;
mod models;
mod pages;
mod router;
mod store;

use crate::{
    components::footer::Footer,
    router::{switch, Route},
};
use api::*;
use components::navbar::Navbar;
use models::*;
use store::Store;
use stylist::yew::Global;
use yew::{function_component, html, Html};
use yew_router::{BrowserRouter, Switch};

const STYLE: &str = include_str!("theme.css");

fn main() {
    yew::Renderer::<App>::new().render();
}

#[function_component(App)]
pub fn app() -> Html {
    html! {
        <>
            <Global css={STYLE} />
            <BrowserRouter>
                <Navbar />
                <Switch<Route> render={switch} />
                <Footer />
            </BrowserRouter>
        </>
    }
}
