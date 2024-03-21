use crate::{
    components::{pager::Pager, post::Post},
    helpers::{self, CustomCallback},
    router::{PostQuery, Route},
    store::Store,
    Api, PostModel,
};
use gloo_net::http::Method;
use stylist::Style;
use yew::prelude::*;
use yew_router::{
    components::Link,
    hooks::{use_location, use_navigator},
};
use yewdux::use_store_value;

const STYLE: &str = include_str!("styles/posts.css");

#[function_component(Posts)]
pub fn posts() -> Html {
    let style = Style::new(STYLE).unwrap();
    let store = use_store_value::<Store>();
    let loading = use_state(|| true);
    let page_num = use_state(|| 1i32);
    let posts = use_state(|| vec![PostModel::default()]);
    let posts_count = use_state(|| i32::default());
    let pages_count = use_state(|| i32::default());
    let trigger = use_state(|| false);
    let posts_cb = CustomCallback::new(&posts);
    let posts_count_cb = CustomCallback::new(&posts_count);
    let pages_count_cb = CustomCallback::new(&pages_count);
    let location = use_location();
    let navigator = use_navigator();

    // get pages count and posts count
    let get_count = |posts_ct_cb: Callback<i32>, pages_ct_cb: Callback<i32>| {
        wasm_bindgen_futures::spawn_local(async move {
            let res = Api::GetPostsCount.fetch(None, None, Method::GET).await;
            helpers::emit(&posts_ct_cb, res.unwrap()).await;

            let res = Api::GetPagesCount.fetch(None, None, Method::GET).await;
            helpers::emit(&pages_ct_cb, res.unwrap()).await;
        });
    };

    let posts_count_cb_clone = posts_count_cb.clone();
    let pages_count_cb_clone = pages_count_cb.clone();
    use_effect_with((), move |_| {
        get_count(posts_count_cb_clone, pages_count_cb_clone);
    });

    // get posts for current page
    let page_num_clone = page_num.clone();
    let loading_clone = loading.clone();
    let location = location.clone();
    use_effect_with(page_num_clone.clone(), move |_| {
        wasm_bindgen_futures::spawn_local(async move {
            loading_clone.set(true);
            let mut num = *page_num_clone as u32;

            let query = location.unwrap().query::<PostQuery>();
            if let Ok(q) = query {
                num = q.page;
            }

            page_num_clone.set(num as i32);
            let res = Api::GetPage(num).fetch(None, None, Method::GET).await;
            helpers::emit(&posts_cb, res.unwrap()).await;
            loading_clone.set(false);
        });
    });

    // page left / right callback
    let trigger_clone = trigger.clone();
    let on_pager_click = {
        let navigator = navigator.clone().unwrap();
        let page_num = page_num.clone();
        Callback::from(move |page: i32| {
            trigger_clone.set(!*trigger_clone);
            let query = &PostQuery { page: page as u32 };
            let _ = navigator.push_with_query(&Route::Posts, &query);
            page_num.set(page)
        })
    };

    // refresh posts on page if post was deleted
    let trigger_clone = trigger.clone();
    let on_post_delete = {
        Callback::from(move |_| {
            trigger_clone.set(!*trigger_clone);
            get_count(posts_count_cb.clone(), pages_count_cb.clone());
        })
    };

    html! {
        <section class={style}>
            <div class="posts">
                <Pager page_num={*page_num} on_click={&on_pager_click} total_pages={*pages_count}/>

                // CREATE POST BUTTON
                if store.admin {
                    <span class="create-post-btn"><Link<Route> to={Route::AddPost}>{"Create Post"}</Link<Route>></span>
                }

                // ALL POSTS
                if !*loading {
                    {for posts.iter().enumerate().map(|(idx, post)| html! {
                        <Post post={post.clone()} post_number={*posts_count - 5 * (*page_num as i32 - 1) - idx as i32} on_post_delete={&on_post_delete}/>
                    })}
                } else {
                    <h1>{"loading..."}</h1>
                }

                <Pager page_num={*page_num} on_click={&on_pager_click} total_pages={*pages_count}/>
            </div>
        </section>
    }
}
