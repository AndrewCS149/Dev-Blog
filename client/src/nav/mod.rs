use yew::prelude::*;
use yew_router::prelude::*;

use crate::Route;

#[function_component(Nav)]
pub fn nav() -> Html {
    html! {
        <div>
            <Link<Route> to={Route::Home}>{"Home"}</Link<Route>>
            <Link<Route> to={Route::Posts}>{"Posts"}</Link<Route>>
            <Link<Route> to={Route::About}>{"About"}</Link<Route>>
        </div>
    }
}
