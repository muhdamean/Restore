import { useState, useEffect } from "react";
import { Product } from "../../models/product";
import ProductList from "./ProductList";

// interface Props{
//     products: Product[];
//     addProduct: ()=>void;
// }

export default function Catalog(){
    const [products, setProducts]=useState<Product[]>([]);

    useEffect(()=>{
      fetch('http://localhost:5000/api/products')
        .then(response=>response.json())
        .then(data=>setProducts(data))
    }, [])
  
    // function addProduct(){
    //   setProducts(prevState=>[...prevState, 
    //     {
    //       id: prevState.length+101,
    //       name:'product' +(prevState.length+1), 
    //       price:(prevState.length*100)+100,
    //       brand: 'some brand',
    //       description:'some description',
    //       pictureUrl:'https://picsum.photos/200'
    //     }])
    // }

    return(
       <>
            <ProductList products={products}></ProductList>
            {/* <Button variant='contained' onClick={addProduct}>Add Product</Button> */}
       </>
    )
}